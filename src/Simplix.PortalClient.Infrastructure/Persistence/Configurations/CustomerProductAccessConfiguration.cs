using Simplix.PortalClient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simplix.PortalClient.Infrastructure.Persistence.Configurations;

internal sealed class CustomerProductAccessConfiguration : IEntityTypeConfiguration<CustomerProductAccess>
{
    public void Configure(EntityTypeBuilder<CustomerProductAccess> builder)
    {
        builder.ToTable("customer_product_accesses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.EnvironmentName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.AccessLabel).HasMaxLength(120).IsRequired();
        builder.Property(x => x.AccessUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.AccessStatus).HasMaxLength(60).IsRequired();
        builder.Property(x => x.CredentialHint).HasMaxLength(250).IsRequired();

        builder.HasIndex(x => x.CustomerEmail);
    }
}

