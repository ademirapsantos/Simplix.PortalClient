namespace Simplix.PortalClient.Application.Contracts.Admin;

public sealed record SystemUpdateTriggerResultViewModel(
    bool Success,
    bool Started,
    string Message);
