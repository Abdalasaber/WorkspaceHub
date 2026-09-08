using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.DTOs.WorkspacePricing
{
    public class CreateWorkspacePricingRequest
    {
        public PricingType PricingType { get; set; }

        public decimal Price { get; set; }

        public int WorkspaceId { get; set; }
    }
}
