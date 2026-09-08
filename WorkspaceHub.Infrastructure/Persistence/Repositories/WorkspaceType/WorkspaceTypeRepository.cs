using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;
using WorkspaceHub.Infrastructure.Persistence.Repositories;
using WorkspaceTypeHub.Application.Interfaces.Repository;

namespace WorkspaceTypeHub.Infrastructure.Persistence.Repositories
{
    public class WorkspaceTypeRepository
        : GenericRepository<WorkspaceType>, IWorkspaceTypeRepository
    {
        private readonly AppDbContext _context;

        public WorkspaceTypeRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<WorkspaceType>> GetAllWithRelationAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkspaceTypes
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<WorkspaceType?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkspaceTypes
                .AsNoTracking()
                .Include(x => x.Tenant)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
        }
    }
}