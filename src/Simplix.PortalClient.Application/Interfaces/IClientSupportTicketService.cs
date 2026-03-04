using Simplix.PortalClient.Application.Contracts.Client;

namespace Simplix.PortalClient.Application.Interfaces;

public interface IClientSupportTicketService
{
    Task<ClientSupportTicketBoardViewModel> GetBoardAsync(string userEmail, bool canManageWorkflow, CancellationToken cancellationToken);
    Task<ClientSupportTicketDetailsViewModel?> GetTicketAsync(Guid ticketId, string userEmail, bool canManageWorkflow, CancellationToken cancellationToken);
    Task CreateTicketAsync(string userEmail, CreateClientSupportTicketCommand command, CancellationToken cancellationToken);
    Task MoveTicketAsync(Guid ticketId, string targetStatus, string userEmail, bool canManageWorkflow, CancellationToken cancellationToken);
}

