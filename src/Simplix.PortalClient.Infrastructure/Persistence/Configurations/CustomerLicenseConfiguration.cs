using Simplix.PortalClient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simplix.PortalClient.Infrastructure.Persistence.Configurations;

internal sealed class CustomerLicenseConfiguration : IEntityTypeConfiguration<CustomerLicense>
{
    public void Configure(EntityTypeBuilder<CustomerLicense> builder)
    {
        builder.ToTable("customer_licenses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.PlanName).HasMaxLength(120).IsRequired();

        builder.HasIndex(x => x.CustomerEmail);
    }
}

