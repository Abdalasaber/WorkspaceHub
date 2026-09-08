namespace WorkspaceHub.Application.DTOs.Branch
{
    public class CreateBranchRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? AddressLine { get; set; }

        public string? GoogleMapsUrl { get; set; }
    }
}
