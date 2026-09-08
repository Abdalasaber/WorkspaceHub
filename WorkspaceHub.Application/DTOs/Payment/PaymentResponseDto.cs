using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.Payment;

public class PaymentResponseDto
{
    public int BookingId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal TotalReceived { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal TotalOverpayment { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public IReadOnlyList<PaymentItemResponseDto> Payments { get; set; } = [];
}