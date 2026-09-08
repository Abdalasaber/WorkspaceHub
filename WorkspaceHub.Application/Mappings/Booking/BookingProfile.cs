using AutoMapper;
using WorkspaceHub.Application.DTOs.Booking;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.TenantName,
                opt => opt.MapFrom(
                    src => src.Tenant.Name))
                .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(
                    src => src.Customer.FullName));

            CreateMap<CreateBookingRequest, Booking>();
            CreateMap<UpdateBookingRequest, Booking>();

        }
    }
}
