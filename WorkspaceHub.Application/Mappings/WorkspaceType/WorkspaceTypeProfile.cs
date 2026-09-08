using AutoMapper;
using WorkspaceHub.Application.DTOs.WorkspaceType;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceTypeHub.Application.Mappings
{
    public class WorkspaceTypeProfile : Profile
    {
        public WorkspaceTypeProfile()
        {
            CreateMap<WorkspaceType, WorkspaceTypeResponseDto>()
                .ForMember(
                dest => dest.TenantName,
                oPT => oPT.MapFrom(src => src.Tenant.Name)
                );

            //CreateMap<WorkspaceType, WorkspaceTypeDetailsResponseDto>();

            CreateMap<CreateWorkspaceTypeRequest, WorkspaceType>();
            CreateMap<UpdateWorkspaceTypeRequest, WorkspaceType>();
        }
    }
}
