using KMKKM.PortalClient.Application.Contracts.Client;
using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Domain.Constants;
using KMKKM.PortalClient.Domain.Entities;
using KMKKM.PortalClient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class ClientWorkspaceService : IClientWorkspaceService
{
    private readonly PortalClientDbContext _dbContext;

    public ClientWorkspaceService(PortalClientDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClientWorkspaceViewModel> GetWorkspaceAsync(string userEmail, bool canViewAll, CancellationToken cancellationToken)
    {
        IQueryable<CustomerLicense> licenseQuery = _dbContext.CustomerLicenses.AsNoTracking();
        IQueryable<CustomerFinancialRecord> financialQuery = _dbContext.CustomerFinancialRecords.AsNoTracking();
        IQueryable<CustomerProductAccess> accessQuery = _dbContext.CustomerProductAccesses.AsNoTracking();
        IQueryable<SupportTicket> ticketQuery = _dbContext.SupportTickets.AsNoTracking();

        if (!canViewAll)
        {
            licenseQuery = licenseQuery.Where(x => x.CustomerEmail == userEmail);
            financialQuery = financialQuery.Where(x => x.CustomerEmail == userEmail);
            accessQuery = accessQuery.Where(x => x.CustomerEmail == userEmail);
            ticketQuery = ticketQuery.Where(x => x.CustomerEmail == userEmail);
        }

        var primaryLicense = await licenseQuery
            .OrderBy(x => x.CustomerName)
            .FirstAsync(cancellationToken);

        var licenses = await licenseQuery
            .Where(x => x.CustomerEmail == primaryLicense.CustomerEmail)
            .OrderBy(x => x.ExpiresOn)
            .Select(x => new ClientLicenseViewModel(
                x.ProductName,
                x.PlanName,
                x.ExpiresOn,
                x.ExpiresOn < DateOnly.FromDateTime(DateTime.UtcNow) ? "Expirada" : "Ativa"))
            .ToArrayAsync(cancellationToken);

        string[] licensedProducts = licenses
            .Select(x => x.ProductName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var availableUpgrades = await _dbContext.Products
            .AsNoTracking()
            .Where(x => x.SupportsUpgrade)
            .Where(x => !licensedProducts.Contains(x.Name))
            .OrderBy(x => x.Name)
            .Select(x => $"Upgrade para {x.Name}")
            .ToArrayAsync(cancellationToken);

        var financialHistory = await financialQuery
            .Where(x => x.CustomerEmail == primaryLicense.CustomerEmail)
            .OrderByDescending(x => x.DueOn)
            .Select(x => new ClientFinancialEntryViewModel(
                x.ReferenceNumber,
                x.ProductName,
                x.Description,
                x.Amount,
                x.DueOn,
                x.PaidOn,
                x.Status,
                x.PaymentMethod))
            .Take(6)
            .ToArrayAsync(cancellationToken);

        var productAccesses = await accessQuery
            .Where(x => x.CustomerEmail == primaryLicense.CustomerEmail)
            .OrderBy(x => x.ProductName)
            .ThenBy(x => x.EnvironmentName)
            .Select(x => new ClientProductAccessViewModel(
                x.ProductName,
                x.EnvironmentName,
                x.AccessLabel,
                x.AccessUrl,
                x.AccessStatus,
                x.CredentialHint))
            .ToArrayAsync(cancellationToken);

        var recentTickets = await ticketQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ClientTicketViewModel(
                x.Subject,
                x.Category,
                x.Status,
                x.CreatedAtUtc))
            .Take(5)
            .ToArrayAsync(cancellationToken);

        int openTickets = recentTickets.Count(x => !string.Equals(x.Status, SupportTicketStatuses.Completed, StringComparison.OrdinalIgnoreCase));
        int openFinancialItems = financialHistory.Count(x => !string.Equals(x.Status, "Pago", StringComparison.OrdinalIgnoreCase));
        decimal openFinancialAmount = financialHistory
            .Where(x => !string.Equals(x.Status, "Pago", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Amount);
        ClientLicenseViewModel nextRenewal = licenses.OrderBy(x => x.ExpiresOn).First();
        int daysToRenewal = nextRenewal.ExpiresOn.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

        ClientMetricViewModel[] metrics =
        [
            new("Licencas ativas", licenses.Count(x => x.Status == "Ativa").ToString(), "Produtos com contrato vigente."),
            new("Proxima renovacao", nextRenewal.ExpiresOn.ToString("dd/MM/yyyy"), $"{daysToRenewal} dias para a renovacao mais proxima."),
            new("Financeiro em aberto", openFinancialAmount.ToString("C"), $"{openFinancialItems} lancamento(s) aguardando baixa ou pagamento."),
            new("Acessos liberados", productAccesses.Length.ToString(), "Entradas prontas para uso nos produtos contratados.")
        ];

        List<string> recommendedActions =
        [
            $"Revisar a renovacao do plano {nextRenewal.PlanName} ate {nextRenewal.ExpiresOn:dd/MM/yyyy}.",
            $"Acompanhar {openTickets} chamado(s) com status ainda aberto ou em analise."
        ];

        if (openFinancialItems > 0)
        {
            recommendedActions.Add($"Conciliar {openFinancialItems} item(ns) do historico financeiro para manter a operacao sem bloqueios.");
        }

        if (availableUpgrades.Length > 0)
        {
            recommendedActions.Add($"Avaliar {availableUpgrades[0].ToLowerInvariant()} para ampliar a capacidade da operacao.");
        }

        var viewModel = new ClientWorkspaceViewModel(
            primaryLicense.CustomerName,
            primaryLicense.PlanName,
            nextRenewal.ExpiresOn,
            metrics,
            licenses,
            financialHistory,
            productAccesses,
            recentTickets,
            recommendedActions,
            availableUpgrades);

        return viewModel;
    }
}
