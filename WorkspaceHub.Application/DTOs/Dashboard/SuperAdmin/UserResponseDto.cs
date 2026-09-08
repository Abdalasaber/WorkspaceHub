namespace WorkspaceHub.Application.DTOs.User
{

    public class UserResponseDto
    {
        public string Id { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;

        public int? TenantId { get; set; }

        public string? TenantName { get; set; }

        public bool IsActive { get; set; }
    }
}