using Simplix.PortalClient.Application.Contracts.Portal;

namespace Simplix.PortalClient.Application.Interfaces;

public interface IPortalExperienceService
{
    Task<LandingPageViewModel> GetLandingPageAsync(CancellationToken cancellationToken);
    Task<StorePageViewModel> GetStorePageAsync(CancellationToken cancellationToken);
    Task RegisterCommercialLeadAsync(CreateCommercialLeadCommand command, CancellationToken cancellationToken);
}

