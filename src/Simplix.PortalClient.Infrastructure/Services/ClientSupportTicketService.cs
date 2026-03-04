using Simplix.PortalClient.Application.Contracts.Client;
using Simplix.PortalClient.Application.Interfaces;
using Simplix.PortalClient.Domain.Constants;
using Simplix.PortalClient.Domain.Entities;
using Simplix.PortalClient.Infrastructure.Identity;
using Simplix.PortalClient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Simplix.PortalClient.Infrastructure.Services;

internal sealed class ClientSupportTicketService : IClientSupportTicketService
{
    private readonly PortalClientDbContext _dbContext;

    public ClientSupportTicketService(PortalClientDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClientSupportTicketBoardViewModel> GetBoardAsync(
        string userEmail,
        bool canManageWorkflow,
        CancellationToken cancellationToken)
    {
        ApplicationUser currentUser = await _dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.Email == userEmail, cancellationToken);

        IQueryable<SupportTicket> query = BuildScopedQuery(userEmail, canManageWorkflow);
        SupportTicket[] tickets = await query
            .OrderByDescending(x => x.LastMovedAtUtc)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var cardsByStatus = tickets
            .GroupBy(x => x.Status)
            .ToDictionary(
                x => x.Key,
                x => (IReadOnlyCollection<ClientSupportTicketCardViewModel>)x
                    .Select(ticket => MapCard(ticket))
                    .ToArray(),
                StringComparer.Ordinal);

        ClientSupportTicketColumnViewModel[] columns = SupportTicketStatuses.Ordered
            .Select(status => new ClientSupportTicketColumnViewModel(
                status,
                status,
                GetColumnDescription(status),
                cardsByStatus.GetValueOrDefault(status, Array.Empty<ClientSupportTicketCardViewModel>())))
            .ToArray();

        return new ClientSupportTicketBoardViewModel(
            currentUser.FullName,
            canManageWorkflow,
            columns);
    }

    public async Task<ClientSupportTicketDetailsViewModel?> GetTicketAsync(
        Guid ticketId,
        string userEmail,
        bool canManageWorkflow,
        CancellationToken cancellationToken)
    {
        SupportTicket? ticket = await BuildScopedQuery(userEmail, canManageWorkflow)
            .FirstOrDefaultAsync(x => x.Id == ticketId, cancellationToken);

        return ticket is null ? null : MapDetails(ticket);
    }

    public async Task CreateTicketAsync(
        string userEmail,
        CreateClientSupportTicketCommand command,
        CancellationToken cancellationToken)
    {
        if (!SupportTicketWorkflow.IsValidCategory(command.Category))
        {
            throw new InvalidOperationException("Categoria de chamado invalida.");
        }

        if (!SupportTicketWorkflow.IsValidPriority(command.Priority))
        {
            throw new InvalidOperationException("Prioridade de chamado invalida.");
        }

        ApplicationUser currentUser = await _dbContext.Users
            .FirstAsync(x => x.Email == userEmail, cancellationToken);

        SupportTicket ticket = new()
        {
            ReferenceNumber = await GenerateReferenceNumberAsync(cancellationToken),
            Subject = command.Subject.Trim(),
            Description = command.Description.Trim(),
            Category = command.Category,
            Priority = command.Priority,
            Status = SupportTicketStatuses.Created,
            CustomerName = currentUser.FullName,
            CustomerEmail = currentUser.Email ?? userEmail,
            CreatedByEmail = currentUser.Email ?? userEmail,
            AssignedAgentFullName = "Aguardando triagem",
            CreatedAtUtc = DateTime.UtcNow,
            LastMovedAtUtc = DateTime.UtcNow,
            SlaTargetAtUtc = DateTime.UtcNow.AddHours(48)
        };

        _dbContext.SupportTickets.Add(ticket);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MoveTicketAsync(
        Guid ticketId,
        string targetStatus,
        string userEmail,
        bool canManageWorkflow,
        CancellationToken cancellationToken)
    {
        if (!SupportTicketWorkflow.IsValidStatus(targetStatus))
        {
            throw new InvalidOperationException("Coluna de destino invalida.");
        }

        SupportTicket? ticket = await BuildScopedQuery(userEmail, canManageWorkflow)
            .FirstOrDefaultAsync(x => x.Id == ticketId, cancellationToken);

        if (ticket is null)
        {
            throw new InvalidOperationException("Chamado nao encontrado para o usuario atual.");
        }

        if (canManageWorkflow)
        {
            ticket.Status = targetStatus;
            ticket.LastMovedAtUtc = DateTime.UtcNow;
        }
        else
        {
            if (!SupportTicketWorkflow.CanClientMove(ticket.Status, targetStatus))
            {
                throw new InvalidOperationException("O cliente nao pode mover este chamado para a coluna selecionada.");
            }

            ticket.Status = targetStatus;
            ticket.LastMovedAtUtc = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<SupportTicket> BuildScopedQuery(string userEmail, bool canManageWorkflow)
    {
        IQueryable<SupportTicket> query = _dbContext.SupportTickets;

        if (!canManageWorkflow)
        {
            query = query.Where(x => x.CustomerEmail == userEmail);
        }

        return query;
    }

    private static ClientSupportTicketCardViewModel MapCard(SupportTicket ticket) =>
        new(
            ticket.Id,
            ticket.ReferenceNumber,
            ticket.Subject,
            ticket.Category,
            ticket.Priority,
            ticket.Status,
            ticket.CustomerName,
            ticket.AssignedAgentFullName,
            ticket.SlaTargetAtUtc,
            BuildSlaLabel(ticket.SlaTargetAtUtc),
            BuildSlaTone(ticket.SlaTargetAtUtc),
            ticket.CreatedAtUtc,
            ticket.LastMovedAtUtc,
            SupportTicketWorkflow.CanClientMove(ticket.Status, SupportTicketStatuses.Completed));

    private static ClientSupportTicketDetailsViewModel MapDetails(SupportTicket ticket) =>
        new(
            ticket.Id,
            ticket.ReferenceNumber,
            ticket.Subject,
            ticket.Description,
            ticket.Category,
            ticket.Priority,
            ticket.Status,
            ticket.CustomerName,
            ticket.CustomerEmail,
            ticket.AssignedAgentFullName,
            ticket.SlaTargetAtUtc,
            BuildSlaLabel(ticket.SlaTargetAtUtc),
            BuildSlaTone(ticket.SlaTargetAtUtc),
            ticket.CreatedAtUtc,
            ticket.LastMovedAtUtc,
            SupportTicketWorkflow.CanClientMove(ticket.Status, SupportTicketStatuses.Completed));

    private static string GetColumnDescription(string status) => status switch
    {
        SupportTicketStatuses.Created => "Chamados novos criados pelo cliente.",
        SupportTicketStatuses.Prioritized => "Fila de urgencia definida pelo suporte.",
        SupportTicketStatuses.Analyst => "Analise de regra de negocio e entendimento.",
        SupportTicketStatuses.Development => "Correcao de codigo e desenvolvimento tecnico.",
        SupportTicketStatuses.Customization => "Implementacao de nova regra, processo ou adaptacao.",
        SupportTicketStatuses.Homologation => "Momento em que o cliente valida a entrega.",
        SupportTicketStatuses.Blocked => "Aguardando dependencia externa ou informacao de terceiros.",
        SupportTicketStatuses.Deploy => "Itens aprovados e aguardando publicacao.",
        SupportTicketStatuses.Completed => "Fluxo encerrado e chamado concluido.",
        _ => string.Empty
    };

    private async Task<string> GenerateReferenceNumberAsync(CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            string candidate = RandomNumberGenerator.GetInt32(10_000_000, 100_000_000).ToString();
            bool exists = await _dbContext.SupportTickets
                .AsNoTracking()
                .AnyAsync(x => x.ReferenceNumber == candidate, cancellationToken);

            if (!exists)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Nao foi possivel gerar um numero unico para o chamado.");
    }

    private static string BuildSlaLabel(DateTime slaTargetAtUtc)
    {
        TimeSpan remaining = slaTargetAtUtc - DateTime.UtcNow;

        if (remaining.TotalMinutes < 0)
        {
            return $"Atrasado ha {Math.Abs((int)Math.Ceiling(remaining.TotalHours))}h";
        }

        if (remaining.TotalHours <= 24)
        {
            return $"Vence em {(int)Math.Ceiling(remaining.TotalHours)}h";
        }

        return $"Dentro do prazo ({(int)Math.Ceiling(remaining.TotalDays)}d)";
    }

    private static string BuildSlaTone(DateTime slaTargetAtUtc)
    {
        TimeSpan remaining = slaTargetAtUtc - DateTime.UtcNow;

        if (remaining.TotalMinutes < 0)
        {
            return "late";
        }

        if (remaining.TotalHours <= 24)
        {
            return "warning";
        }

        return "ok";
    }
}

