using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.Payment;

public class PaymentItemResponseDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public decimal AppliedAmount { get; set; }

    public decimal OverpaymentAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? TransactionId { get; set; }

    public string? Notes { get; set; }

    public int BookingId { get; set; }
}