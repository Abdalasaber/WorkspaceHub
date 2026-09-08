using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Infrastructure.Persistence.Context;

namespace WorkspaceHub.Infrastructure.Persistence.Repositories;

public class BookingRepository
    : GenericRepository<Booking>, IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Booking>> GetAllWithRelationAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Include(x => x.Customer)
            .Include(x => x.Workspace)
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetByIdWithRelationAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Include(x => x.Customer)
            .Include(x => x.Workspace)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId,
                cancellationToken);
    }
}