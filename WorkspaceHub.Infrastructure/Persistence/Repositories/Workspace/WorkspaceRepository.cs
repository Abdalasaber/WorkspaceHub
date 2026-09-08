using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;

namespace WorkspaceHub.Infrastructure.Persistence.Repositories
{

    public class WorkspaceRepository
        : GenericRepository<Workspace>, IWorkspaceRepository
    {
        private readonly AppDbContext _context;

        public WorkspaceRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Workspace>> GetAllWithRelationAsync(
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Workspaces
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Include(x => x.Branch)
                .Include(x => x.WorkspaceType)
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Workspace?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Workspaces
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Include(x => x.Branch)
                .Include(x => x.WorkspaceType)
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.TenantId == tenantId,
                    cancellationToken);
        }
    }
}
