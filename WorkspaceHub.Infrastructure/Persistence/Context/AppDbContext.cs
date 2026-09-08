using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Domain.Common.Base;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Infrastructure.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>, IUnitOfWork
    {

        private readonly ICurrentUserService _currentUserService;
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService
            ) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<WorkspaceType> WorkspaceTypes { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspacePricing> WorkspacePricings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

            ApplySoftDeleteQueryFilters(modelBuilder);

        }
        private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(x =>
                    typeof(SoftDeleteEntity).IsAssignableFrom(x.ClrType)))
            {
                var parameter = Expression.Parameter(
                    entityType.ClrType,
                    "e");
                var property = Expression.Property(
                    parameter,
                    nameof(SoftDeleteEntity.IsDeleted));
                var falseValue = Expression.Constant(false);
                var body = Expression.Equal(
                    property,
                    falseValue);
                var lambda = Expression.Lambda(
                    body,
                    parameter);
                entityType.SetQueryFilter(lambda);
            }
        }

        public override async Task<int> SaveChangesAsync(
           CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();

            ApplyAudit();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit()
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();
            var userId = _currentUserService.UserId;
            var now = DateTime.UtcNow;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                }
            }
        }

        private void ApplySoftDelete()
        {
            var entries = ChangeTracker
                .Entries<SoftDeleteEntity>()
                .Where(x => x.State == EntityState.Deleted);
            var userId = _currentUserService.UserId;
            var now = DateTime.UtcNow;
            foreach (var entry in entries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
                entry.Entity.DeletedBy = userId;
            }
        }
    }
}
