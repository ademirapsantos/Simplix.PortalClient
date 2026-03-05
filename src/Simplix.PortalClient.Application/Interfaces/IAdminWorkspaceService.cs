using Simplix.PortalClient.Application.Contracts.Admin;

namespace Simplix.PortalClient.Application.Interfaces;

public interface IAdminWorkspaceService
{
    Task<AdminWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken);
    Task<SystemUpdateTriggerResultViewModel> StartSystemUpdateAsync(CancellationToken cancellationToken);
    Task<SystemUpdateRuntimeStatusViewModel> GetSystemUpdateRuntimeStatusAsync(CancellationToken cancellationToken);
}
