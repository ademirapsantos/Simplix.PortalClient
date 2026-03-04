namespace Simplix.PortalClient.Domain.Entities;

public sealed class CustomerProductAccess
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = string.Empty;
    public string AccessLabel { get; set; } = string.Empty;
    public string AccessUrl { get; set; } = string.Empty;
    public string AccessStatus { get; set; } = string.Empty;
    public string CredentialHint { get; set; } = string.Empty;
}

