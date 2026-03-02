namespace KMKKM.PortalClient.Domain.Entities;

public sealed class SupportTicket
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Subject { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
