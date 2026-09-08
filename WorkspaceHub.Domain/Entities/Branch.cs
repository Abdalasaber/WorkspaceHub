using WorkspaceHub.Domain.Common.Base;

namespace WorkspaceHub.Domain.Entities;

public class Branch : TenantEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AddressLine { get; set; }

    public string? GoogleMapsUrl { get; set; }

    public bool IsActive { get; set; } = true;

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    #endregion
}