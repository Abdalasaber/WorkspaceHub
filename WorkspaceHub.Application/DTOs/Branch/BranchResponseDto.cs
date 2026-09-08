namespace WorkspaceHub.Application.DTOs.Branch
{
    public class BranchResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? AddressLine { get; set; }

        public string? GoogleMapsUrl { get; set; }

        public bool IsActive { get; set; }

        public string TenantName { get; set; }
        public int TenantId { get; set; }
    }
}
