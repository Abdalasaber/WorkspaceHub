using AutoMapper;
using WorkspaceHub.Application.DTOs.Tenant;
using WorkspaceHub.Domain.Entities;

namespace TenantHub.Application.Mappings
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<Tenant, TenantResponseDto>();

            CreateMap<CreateTenantRequest, Tenant>();
            CreateMap<UpdateTenantRequest, Tenant>();
            CreateMap<UpdateCurrentTenantRequest, Tenant>();
        }
    }
}
