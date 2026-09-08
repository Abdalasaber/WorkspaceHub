using AutoMapper;
using WorkspaceHub.Application.DTOs.Payment;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentResponseDto>();
            CreateMap<Payment, PaymentItemResponseDto>();


            CreateMap<CreatePaymentRequest, Payment>();
        }
    }
}
