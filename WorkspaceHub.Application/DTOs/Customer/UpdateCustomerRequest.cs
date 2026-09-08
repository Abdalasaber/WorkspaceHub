namespace WorkspaceHub.Application.DTOs.Customer
{
    public class UpdateCustomerRequest
    {
        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? Email { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; }
    }
}
