using WorkspaceHub.Domain.Common.Base;

namespace WorkspaceHub.Domain.Entities;

public class Workspace : TenantEntity
{
    public string Name { get; set; } = null!;

    public string Code { get; set; }

    public string? Description { get; set; }

    public int Capacity { get; set; }

    public bool IsAvailable { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public int BranchId { get; set; }

    public int WorkspaceTypeId { get; set; }

    #region Navigation Properties

    public Tenant Tenant { get; set; } = null!;

    public Branch Branch { get; set; } = null!;

    public WorkspaceType WorkspaceType { get; set; } = null!;

    public ICollection<WorkspacePricing> WorkspacePricings { get; set; } = new List<WorkspacePricing>();

    #endregion
}