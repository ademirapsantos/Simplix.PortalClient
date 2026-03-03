using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KMKKM.PortalClient.Infrastructure.Persistence;

public sealed class PortalClientDbContextFactory : IDesignTimeDbContextFactory<PortalClientDbContext>
{
    public PortalClientDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PortalClientDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=kmkkm_portal_client;Username=postgres;Password=postgres");

        return new PortalClientDbContext(optionsBuilder.Options);
    }
}
