using WorkspaceHub.Domain.Common.Base;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Domain.Entities;

public class Booking : TenantEntity
{
    public int CustomerId { get; set; }

    public int WorkspaceId { get; set; }

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

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    public Customer Customer { get; set; } = null!;

    public Workspace Workspace { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    #endregion
}