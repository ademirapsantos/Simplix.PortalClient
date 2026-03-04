using Simplix.PortalClient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Simplix.PortalClient.Infrastructure.Persistence.Configurations;

internal sealed class CommercialPlanConfiguration : IEntityTypeConfiguration<CommercialPlan>
{
    public void Configure(EntityTypeBuilder<CommercialPlan> builder)
    {
        builder.ToTable("commercial_plans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.TargetAudience).HasMaxLength(200).IsRequired();
    }
}

