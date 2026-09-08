using AutoMapper;
using WorkspaceHub.Application.DTOs.Workspace;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Mappings
{
    public class WprkspaceProfile : Profile
    {
        public WprkspaceProfile()
        {
            CreateMap<Workspace, WorkspaceResponseDto>()
                .ForMember(dest => dest.TenantName,
                o => o.MapFrom(src => src.Tenant.Name))

                .ForMember(dest => dest.BranchName,
                o => o.MapFrom(src => src.Branch.Name))

                .ForMember(dest => dest.WorkspaceType,
                o => o.MapFrom(src => src.WorkspaceType.Name));

            CreateMap<CreateWorkspaceRequest, Workspace>();
            CreateMap<UpdateWorkspaceRequest, Workspace>();
        }
    }
}
