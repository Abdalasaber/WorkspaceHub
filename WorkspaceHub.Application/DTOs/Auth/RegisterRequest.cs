namespace WorkspaceHub.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string TenantName { get; set; } = null!;
    public string TenantSlug { get; set; } = null!;
}