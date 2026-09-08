using WorkspaceHub.Domain.Common.Base;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Domain.Entities;

public class Payment : TenantEntity
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public decimal AppliedAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public string? TransactionId { get; set; }

    public string? Notes { get; set; }

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    public Booking Booking { get; set; } = null!;

    #endregion
}