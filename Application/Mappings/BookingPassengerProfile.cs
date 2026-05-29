using AutoMapper;
using TouRest.Application.DTOs.Booking;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class BookingPassengerProfile : Profile
    {
        public BookingPassengerProfile()
        {
            CreateMap<BookingPassenger, BookingPassengerDTO>();
            CreateMap<PassengerRequest, BookingPassenger>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.BookingId, opt => opt.Ignore());
        }
    }
}
