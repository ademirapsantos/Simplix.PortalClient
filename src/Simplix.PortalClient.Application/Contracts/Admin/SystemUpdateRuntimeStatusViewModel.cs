namespace Simplix.PortalClient.Application.Contracts.Admin;

public sealed record SystemUpdateRuntimeStatusViewModel(
    bool InProgress,
    string CurrentStage,
    string? LastError,
    string? TargetVersion,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? FinishedAtUtc);
