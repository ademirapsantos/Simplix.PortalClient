using KMKKM.PortalClient.Infrastructure.Identity;
using KMKKM.PortalClient.Domain.Entities;
using KMKKM.PortalClient.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KMKKM.PortalClient.Infrastructure.Persistence;

public sealed class PortalClientDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public PortalClientDbContext(DbContextOptions<PortalClientDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<CommercialPlan> CommercialPlans => Set<CommercialPlan>();
    public DbSet<CustomerLicense> CustomerLicenses => Set<CustomerLicense>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CommercialPlanConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerLicenseConfiguration());
        modelBuilder.ApplyConfiguration(new SupportTicketConfiguration());
    }
}
