using Simplix.PortalClient.Infrastructure.Identity;
using Simplix.PortalClient.Domain.Entities;
using Simplix.PortalClient.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Simplix.PortalClient.Infrastructure.Persistence;

public sealed class PortalClientDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public PortalClientDbContext(DbContextOptions<PortalClientDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<CommercialPlan> CommercialPlans => Set<CommercialPlan>();
    public DbSet<CommercialLead> CommercialLeads => Set<CommercialLead>();
    public DbSet<CustomerLicense> CustomerLicenses => Set<CustomerLicense>();
    public DbSet<CustomerFinancialRecord> CustomerFinancialRecords => Set<CustomerFinancialRecord>();
    public DbSet<CustomerProductAccess> CustomerProductAccesses => Set<CustomerProductAccess>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CommercialPlanConfiguration());
        modelBuilder.ApplyConfiguration(new CommercialLeadConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerLicenseConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerFinancialRecordConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerProductAccessConfiguration());
        modelBuilder.ApplyConfiguration(new SupportTicketConfiguration());
    }
}

