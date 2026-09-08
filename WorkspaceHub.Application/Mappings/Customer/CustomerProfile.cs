using AutoMapper;
using WorkspaceHub.Application.DTOs.Customer;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Mappings
{
    class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(
                dest => dest.TenantName,
                opt => opt.MapFrom(
                    src => src.Tenant.Name
                    ));

            CreateMap<CreateCustomerRequest, Customer>();
            CreateMap<UpdateCustomerRequest, Customer>();

        }
    }
}