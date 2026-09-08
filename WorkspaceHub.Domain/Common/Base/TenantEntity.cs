namespace WorkspaceHub.Domain.Common.Base;

public abstract class TenantEntity : SoftDeleteEntity
{
    public int TenantId { get; set; }
}