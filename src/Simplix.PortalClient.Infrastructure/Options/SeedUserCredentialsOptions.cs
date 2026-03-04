namespace Simplix.PortalClient.Infrastructure.Options;

public sealed class SeedUserCredentialsOptions
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

