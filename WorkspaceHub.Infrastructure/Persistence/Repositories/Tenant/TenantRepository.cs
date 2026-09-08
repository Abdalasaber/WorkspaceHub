using Microsoft.EntityFrameworkCore;
using TenantHub.Application.Interfaces.Repository;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;

namespace WorkspaceHub.Infrastructure.Persistence.Repositories
{
    class TenantRepository : GenericRepository<Tenant>
        , ITenantRepository
    {
        protected readonly AppDbContext _context;
        public TenantRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .ToListAsync(cancellationToken);
        }

        public async Task<Tenant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
