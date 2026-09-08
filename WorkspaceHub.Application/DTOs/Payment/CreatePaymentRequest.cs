using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.Payment
{

    public class CreatePaymentRequest
    {
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string? TransactionId { get; set; }

        public string? Notes { get; set; }

        public int BookingId { get; set; }
    }
}