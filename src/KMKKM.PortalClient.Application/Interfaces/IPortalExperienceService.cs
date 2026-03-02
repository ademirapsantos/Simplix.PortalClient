using KMKKM.PortalClient.Application.Contracts.Portal;

namespace KMKKM.PortalClient.Application.Interfaces;

public interface IPortalExperienceService
{
    Task<LandingPageViewModel> GetLandingPageAsync(CancellationToken cancellationToken);
    Task<StorePageViewModel> GetStorePageAsync(CancellationToken cancellationToken);
}
