namespace KMKKM.PortalClient.Infrastructure.Options;

public sealed class OpenClawOptions
{
    public const string SectionName = "OpenClaw";

    public string BaseUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public bool EnableTicketAutomation { get; init; }
}
