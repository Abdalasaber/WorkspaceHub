namespace WorkspaceHub.Application.DTOs.Dashboard
{
    public class CreateStaffRequest
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}