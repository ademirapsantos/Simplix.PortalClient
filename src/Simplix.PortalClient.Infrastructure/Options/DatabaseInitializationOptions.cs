namespace Simplix.PortalClient.Infrastructure.Options;

public sealed class DatabaseInitializationOptions
{
    public const string SectionName = "DatabaseInitialization";

    public bool Enabled { get; init; } = false;
    public bool ApplySchemaChanges { get; init; } = false;
    public bool SeedData { get; init; } = false;
    public bool SeedIdentity { get; init; } = false;
}
