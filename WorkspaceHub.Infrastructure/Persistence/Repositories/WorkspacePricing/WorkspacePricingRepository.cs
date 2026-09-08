using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;

namespace WorkspaceHub.Infrastructure.Persistence.Repositories
{
    public class WorkspacePricingRepository
        : GenericRepository<WorkspacePricing>, IWorkspacePricingRepository
    {
        private readonly AppDbContext _context;

        public WorkspacePricingRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<WorkspacePricing>> GetAllWithRelationAsync(
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkspacePricings
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Include(x => x.Workspace)
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<WorkspacePricing?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkspacePricings
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Include(x => x.Workspace)
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.TenantId == tenantId,
                    cancellationToken);
        }
    }
}