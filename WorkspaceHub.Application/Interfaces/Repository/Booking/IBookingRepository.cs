using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Interfaces.Repository
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking?> GetByIdWithRelationAsync(int id, int tenantId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Booking>> GetAllWithRelationAsync(int tenantId,
            CancellationToken cancellationToken = default);
    }
}
