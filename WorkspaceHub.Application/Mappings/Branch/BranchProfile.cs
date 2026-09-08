using AutoMapper;
using WorkspaceHub.Application.DTOs.Branch;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Mappings
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {
            CreateMap<Branch, BranchResponseDto>()
                .ForMember(
                dest => dest.TenantName,
                opt => opt.MapFrom(
                    src => src.Tenant.Name
                    ));

            CreateMap<CreateBranchRequest, Branch>();
            CreateMap<UpdateBranchRequest, Branch>();

        }
    }
}
