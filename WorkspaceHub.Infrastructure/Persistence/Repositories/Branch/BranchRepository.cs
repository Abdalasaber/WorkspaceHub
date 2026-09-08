using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;

namespace WorkspaceHub.Infrastructure.Persistence.Repositories
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        protected readonly AppDbContext _context;
        public BranchRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Branch>> GetAllWithRelationAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.Branches
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Branch?> GetByIdWithRelationAsync(int id, int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.Branches
                .AsNoTracking()
                .Include(x => x.Tenant)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
        }
    }
}