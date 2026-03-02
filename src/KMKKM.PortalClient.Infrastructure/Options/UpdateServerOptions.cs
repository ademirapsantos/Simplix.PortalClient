namespace KMKKM.PortalClient.Infrastructure.Options;

public sealed class UpdateServerOptions
{
    public const string SectionName = "UpdateServer";

    public string BaseUrl { get; init; } = string.Empty;
    public string CurrentChannel { get; init; } = "stable";
    public bool EnableAutoUpdatePrompt { get; init; } = true;
}
