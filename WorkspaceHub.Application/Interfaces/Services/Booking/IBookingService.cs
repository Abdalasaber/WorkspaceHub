using WorkspaceHub.Application.DTOs.Booking;

namespace WorkspaceHub.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateBookingRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<BookingResponseDto?> GetByIdWithRelationAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BookingResponseDto>> GetAllWithRelationAsync(CancellationToken cancellationToken = default);
    }
}
