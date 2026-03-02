namespace KMKKM.PortalClient.Application.Contracts.Client;

public sealed record ClientWorkspaceViewModel(
    string CustomerName,
    IReadOnlyCollection<string> ActiveProducts,
    IReadOnlyCollection<string> AvailableUpgrades,
    IReadOnlyCollection<string> PendingActions,
    IReadOnlyCollection<string> RecentTickets);
