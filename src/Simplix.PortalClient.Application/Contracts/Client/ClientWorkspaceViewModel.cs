namespace Simplix.PortalClient.Application.Contracts.Client;

public sealed record ClientWorkspaceViewModel(
    string CustomerName,
    string CurrentPlan,
    DateOnly NextRenewalDate,
    IReadOnlyCollection<ClientMetricViewModel> Metrics,
    IReadOnlyCollection<ClientLicenseViewModel> Licenses,
    IReadOnlyCollection<ClientFinancialEntryViewModel> FinancialHistory,
    IReadOnlyCollection<ClientProductAccessViewModel> ProductAccesses,
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

public sealed record ClientFinancialEntryViewModel(
    string ReferenceNumber,
    string ProductName,
    string Description,
    decimal Amount,
    DateOnly DueOn,
    DateOnly? PaidOn,
    string Status,
    string PaymentMethod);

public sealed record ClientProductAccessViewModel(
    string ProductName,
    string EnvironmentName,
    string AccessLabel,
    string AccessUrl,
    string AccessStatus,
    string CredentialHint);

public sealed record ClientTicketViewModel(
    string Subject,
    string Category,
    string Status,
    DateTime CreatedAtUtc);

