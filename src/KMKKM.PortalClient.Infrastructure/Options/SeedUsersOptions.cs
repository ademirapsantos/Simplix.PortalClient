namespace KMKKM.PortalClient.Infrastructure.Options;

public sealed class SeedUsersOptions
{
    public const string SectionName = "SeedUsers";

    public SeedUserCredentialsOptions Admin { get; init; } = new();
    public SeedUserCredentialsOptions Sales { get; init; } = new();
    public SeedUserCredentialsOptions Support { get; init; } = new();
    public SeedUserCredentialsOptions Client { get; init; } = new();
}
