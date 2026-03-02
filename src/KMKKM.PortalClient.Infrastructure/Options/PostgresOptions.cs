namespace KMKKM.PortalClient.Infrastructure.Options;

public sealed class PostgresOptions
{
    public const string SectionName = "ConnectionStrings";

    public string Postgres { get; init; } = string.Empty;
}
