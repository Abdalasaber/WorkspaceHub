using WorkspaceHub.Domain.Common.Base;

namespace WorkspaceHub.Domain.Entities;

public class Customer : TenantEntity
{
    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    #endregion
}