using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.WorkspacePricing
{
    public class UpdateWorkspacePricingRequest
    {
        public PricingType PricingType { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int WorkspaceId { get; set; }
    }
}
