namespace KMKKM.PortalClient.Application.Contracts.Client;

public sealed record ClientWorkspaceViewModel(
    string CustomerName,
    string CurrentPlan,
    DateOnly NextRenewalDate,
    IReadOnlyCollection<ClientMetricViewModel> Metrics,
    IReadOnlyCollection<ClientLicenseViewModel> Licenses,
    IReadOnlyCollection<ClientTicketViewModel> RecentTickets,
    IReadOnlyCollection<string> RecommendedActions,
    IReadOnlyCollection<string> AvailableUpgrades);

public sealed record ClientMetricViewModel(
    string Label,
    string Value,
    string Context);

public sealed record ClientLicenseViewModel(
    string ProductName,
    string PlanName,
    DateOnly ExpiresOn,
    string Status);

public sealed record ClientTicketViewModel(
    string Subject,
    string Category,
    string Status,
    DateTime CreatedAtUtc);
