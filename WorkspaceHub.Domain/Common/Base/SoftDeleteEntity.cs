namespace WorkspaceHub.Domain.Common.Base;

public abstract class SoftDeleteEntity : AuditableEntity
{
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }
}