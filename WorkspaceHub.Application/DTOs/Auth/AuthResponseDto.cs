namespace WorkspaceHub.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int? TenantId { get; set; }
}