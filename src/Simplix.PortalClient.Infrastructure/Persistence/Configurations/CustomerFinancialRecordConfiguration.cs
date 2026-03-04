using Simplix.PortalClient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simplix.PortalClient.Infrastructure.Persistence.Configurations;

internal sealed class CustomerFinancialRecordConfiguration : IEntityTypeConfiguration<CustomerFinancialRecord>
{
    public void Configure(EntityTypeBuilder<CustomerFinancialRecord> builder)
    {
        builder.ToTable("customer_financial_records");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ReferenceNumber).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(60).IsRequired();
        builder.Property(x => x.PaymentMethod).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.HasIndex(x => x.CustomerEmail);
        builder.HasIndex(x => x.DueOn);
    }
}

