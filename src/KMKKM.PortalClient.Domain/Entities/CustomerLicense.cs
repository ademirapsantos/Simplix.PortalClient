namespace KMKKM.PortalClient.Domain.Entities;

public sealed class CustomerLicense
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string PlanName { get; init; } = string.Empty;
    public DateOnly ExpiresOn { get; init; }
}
