namespace Simplix.PortalClient.Application.Contracts.Admin;

public sealed record SystemUpdateStatusViewModel(
    bool IsAvailable,
    bool HasPendingUpdate,
    bool RequiresAdminApproval,
    string CurrentVersion,
    string TargetVersion,
    string Channel,
    string Summary);

