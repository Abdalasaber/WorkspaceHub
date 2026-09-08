using WorkspaceHub.Application.DTOs.Payment;

namespace WorkspaceHub.Application.Interfaces.Services.Payment
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> GetByBookingAsync(int bookingId, CancellationToken cancellationToken = default);

        Task<PaymentResponseDto> CreateAsync(
            CreatePaymentRequest request,
            CancellationToken cancellationToken = default);
        Task<PaymentResponseDto> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
