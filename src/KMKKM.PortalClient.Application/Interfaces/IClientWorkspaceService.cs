using KMKKM.PortalClient.Application.Contracts.Client;

namespace KMKKM.PortalClient.Application.Interfaces;

public interface IClientWorkspaceService
{
    Task<ClientWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken);
}
