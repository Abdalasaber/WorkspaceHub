using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.Booking
{
    public class CreateBookingRequest
    {
        public PricingType PricingType { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public string? Notes { get; set; }

        public int CustomerId { get; set; }

        public int WorkspaceId { get; set; }
    }
}
