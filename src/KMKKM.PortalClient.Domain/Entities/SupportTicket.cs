namespace KMKKM.PortalClient.Domain.Entities;

public sealed class SupportTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CreatedByEmail { get; set; } = string.Empty;
    public string AssignedAgentFullName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastMovedAtUtc { get; set; }
    public DateTime SlaTargetAtUtc { get; set; }
}
