namespace Simplix.PortalClient.Application.Contracts.Admin;

public sealed record OpenClawStatusViewModel(
    string Mode,
    string Summary,
    IReadOnlyCollection<string> NextSteps);

