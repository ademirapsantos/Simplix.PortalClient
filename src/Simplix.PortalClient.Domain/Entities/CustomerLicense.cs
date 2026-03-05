namespace Simplix.PortalClient.Domain.Entities;

public sealed class CustomerLicense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateOnly ExpiresOn { get; set; }
}

