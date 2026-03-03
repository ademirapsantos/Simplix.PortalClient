namespace KMKKM.PortalClient.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Positioning { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool SupportsUpgrade { get; set; }
}
