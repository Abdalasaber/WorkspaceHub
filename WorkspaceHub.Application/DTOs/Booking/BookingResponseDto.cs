using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int Id { get; set; }

        public PricingType PricingType { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string? Notes { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public int WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }
    }
}
