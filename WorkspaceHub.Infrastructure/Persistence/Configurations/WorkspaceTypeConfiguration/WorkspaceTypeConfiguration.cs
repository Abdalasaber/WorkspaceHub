using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Extensions;

namespace WorkspaceHub.Infrastructure.Persistence.Configurations;

public class WorkspaceTypeConfiguration : IEntityTypeConfiguration<WorkspaceType>
{
    public void Configure(EntityTypeBuilder<WorkspaceType> builder)
    {
        builder.ToTable("WorkspaceTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Icon)
            .HasMaxLength(200);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Name
        })
        .HasSoftDeleteUniqueFilter();
    }
}