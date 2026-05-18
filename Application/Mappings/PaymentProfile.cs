using AutoMapper;
using TouRest.Application.DTOs.Payment;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
