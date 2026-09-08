namespace WorkspaceHub.Application.DTOs.Tenant
{
    public class CreateTenantRequest
    {
        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? LogoUrl { get; set; }

        public string? Address { get; set; }
    }
}
