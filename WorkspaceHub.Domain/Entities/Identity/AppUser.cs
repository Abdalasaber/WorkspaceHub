using Microsoft.AspNetCore.Identity;

namespace WorkspaceHub.Domain.Entities;
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int? TenantId { get; set; }

    public Tenant? Tenant { get; set; }
}
