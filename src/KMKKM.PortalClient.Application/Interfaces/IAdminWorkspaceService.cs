using KMKKM.PortalClient.Application.Contracts.Admin;

namespace KMKKM.PortalClient.Application.Interfaces;

public interface IAdminWorkspaceService
{
    Task<AdminWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken);
}
