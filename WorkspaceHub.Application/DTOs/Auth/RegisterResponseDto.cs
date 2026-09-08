namespace WorkspaceHub.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public int TenantId { get; set; }

    public string TenantName { get; set; } = null!;

    public string AdminEmail { get; set; } = null!;

    public string Message { get; set; } = null!;
}