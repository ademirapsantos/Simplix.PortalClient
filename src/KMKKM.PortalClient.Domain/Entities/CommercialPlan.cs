namespace KMKKM.PortalClient.Domain.Entities;

public sealed class CommercialPlan
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TargetAudience { get; init; } = string.Empty;
}
