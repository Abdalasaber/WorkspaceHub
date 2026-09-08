using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.WorkspacePricing
{
    public class WorkspacePricingResponseDto
    {
        public int Id { get; set; }

        public PricingType PricingType { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }
    }
}
