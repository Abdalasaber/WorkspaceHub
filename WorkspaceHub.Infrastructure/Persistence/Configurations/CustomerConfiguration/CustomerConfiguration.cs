using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Extensions;

namespace WorkspaceHub.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.FullName }).HasSoftDeleteUniqueFilter();

        builder.HasIndex(x => new { x.TenantId, x.PhoneNumber }).HasSoftDeleteUniqueFilter();

        builder.HasIndex(x => new { x.TenantId, x.Email }).IsUnique().HasFilter("[IsDeleted] = 0 AND [Email] IS NOT NULL");


    }
}