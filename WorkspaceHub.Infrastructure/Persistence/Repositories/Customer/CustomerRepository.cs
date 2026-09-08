using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;

namespace WorkspaceHub.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository
        : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Customer>> GetAllWithRelationAsync(
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Customer?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(x => x.Tenant)
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.TenantId == tenantId,
                    cancellationToken);
        }
    }
}
