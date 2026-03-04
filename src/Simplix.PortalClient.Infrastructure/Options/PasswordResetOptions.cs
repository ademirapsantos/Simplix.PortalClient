namespace Simplix.PortalClient.Infrastructure.Options;

public sealed class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";

    public bool Enabled { get; init; } = true;
    public bool ExposeResetLinkInResponse { get; init; }
    public int TokenLifespanMinutes { get; init; } = 30;
    public string PublicOrigin { get; init; } = string.Empty;
}

