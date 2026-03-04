namespace KMKKM.PortalClient.Domain.Entities;

public sealed class CommercialLead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ContactName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string InterestedProduct { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public string Status { get; set; } = "Novo";
}
