using Simplix.PortalClient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simplix.PortalClient.Infrastructure.Persistence.Configurations;

internal sealed class CommercialLeadConfiguration : IEntityTypeConfiguration<CommercialLead>
{
    public void Configure(EntityTypeBuilder<CommercialLead> builder)
    {
        builder.ToTable("commercial_leads");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContactName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CompanyName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(40).IsRequired();
        builder.Property(x => x.InterestedProduct).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(40).IsRequired();

        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}

