using KMKKM.PortalClient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KMKKM.PortalClient.Infrastructure.Persistence.Configurations;

internal sealed class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("support_tickets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReferenceNumber).HasMaxLength(8).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Priority).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(80).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(180).IsRequired();
        builder.Property(x => x.CreatedByEmail).HasMaxLength(180).IsRequired();
        builder.Property(x => x.AssignedAgentFullName).HasMaxLength(180).IsRequired();

        builder.HasIndex(x => x.ReferenceNumber).IsUnique();
        builder.HasIndex(x => x.CustomerEmail);
        builder.HasIndex(x => x.Status);
    }
}
