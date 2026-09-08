namespace WorkspaceHub.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        int? TenantId { get; }
        string? FullName { get; }
        string? Role { get; }
        bool IsSuperAdmin { get; }

        int GetRequiredTenantId();
    }
}
