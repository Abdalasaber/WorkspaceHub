namespace WorkspaceHub.Application.DTOs.Customer
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? Email { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

    }
}
