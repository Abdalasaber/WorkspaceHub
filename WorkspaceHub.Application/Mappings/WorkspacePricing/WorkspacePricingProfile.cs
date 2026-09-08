using AutoMapper;
using WorkspaceHub.Application.DTOs.WorkspacePricing;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Mappings
{
    public class WorkspacePricingProfile : Profile
    {
        public WorkspacePricingProfile()
        {
            CreateMap<WorkspacePricing, WorkspacePricingResponseDto>()
                .ForMember(dest => dest.TenantName,
                o => o.MapFrom(src => src.Tenant.Name))
            .ForMember(dest => dest.WorkspaceName,
                o => o.MapFrom(src => src.Workspace.Name));

            CreateMap<CreateWorkspacePricingRequest, WorkspacePricing>();

            CreateMap<UpdateWorkspacePricingRequest, WorkspacePricing>();
        }
    }
}
