namespace KMKKM.PortalClient.Application.Contracts.Admin;

public sealed record AdminWorkspaceViewModel(
    string WelcomeTitle,
    IReadOnlyCollection<AdminMetricViewModel> SalesMetrics,
    IReadOnlyCollection<AdminMetricViewModel> SupportMetrics,
    IReadOnlyCollection<string> PriorityQueues,
    OpenClawStatusViewModel OpenClawStatus);
