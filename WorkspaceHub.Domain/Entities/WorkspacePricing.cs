using WorkspaceHub.Domain.Common.Base;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Domain.Entities;

public class WorkspacePricing : TenantEntity
{
    public int WorkspaceId { get; set; }

    public PricingType PricingType { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    public Workspace Workspace { get; set; } = null!;

    #endregion
}