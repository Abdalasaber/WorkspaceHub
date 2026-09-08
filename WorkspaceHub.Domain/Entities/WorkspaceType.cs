using WorkspaceHub.Domain.Common.Base;

namespace WorkspaceHub.Domain.Entities;

public class WorkspaceType : TenantEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public bool IsActive { get; set; } = true;

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    #endregion
}