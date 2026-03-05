namespace Simplix.PortalClient.Application.Contracts.Portal;

public sealed record LandingPageViewModel(
    string CompanyName,
    string ProductName,
    string CompanySummary,
    string AboutCompany,
    IReadOnlyCollection<string> Highlights,
    IReadOnlyCollection<StoreAppCardViewModel> FeaturedApplications,
    IReadOnlyCollection<CommercialPlanViewModel> CommercialPlans,
    IReadOnlyCollection<RoadmapItemViewModel> Roadmap);

