using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Extensions;

namespace WorkspaceHub.Infrastructure.Persistence.Configurations;

public class WorkspacePricingConfiguration : IEntityTypeConfiguration<WorkspacePricing>
{
    public void Configure(EntityTypeBuilder<WorkspacePricing> builder)
    {
        builder.ToTable("WorkspacePricings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.PricingType)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.WorkspacePricings)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.WorkspaceId,
            x.PricingType
        })
        .HasSoftDeleteUniqueFilter();
    }
}