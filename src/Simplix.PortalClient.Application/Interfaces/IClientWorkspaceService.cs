using Simplix.PortalClient.Application.Contracts.Client;

namespace Simplix.PortalClient.Application.Interfaces;

public interface IClientWorkspaceService
{
    Task<ClientWorkspaceViewModel> GetWorkspaceAsync(string userEmail, bool canViewAll, CancellationToken cancellationToken);
}

