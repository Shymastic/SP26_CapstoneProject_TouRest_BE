using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.BookingItinerary;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class BookingItineraryProfile : Profile
    {
        public BookingItineraryProfile()
        {

            CreateMap<BookingItinerary, BookingItineraryDTO>()
    .ForMember(dest => dest.ItineraryName, opt => opt.MapFrom(src =>
        src.ItinerarySchedule.Itinerary.Name))
    .ForMember(dest => dest.ScheduleStartTime, opt => opt.MapFrom(src =>
        src.ItinerarySchedule.StartTime))
    .ForMember(dest => dest.ScheduleEndTime, opt => opt.MapFrom(src =>
        src.ItinerarySchedule.EndTime))
    .ForMember(dest => dest.VoucherCode, opt => opt.MapFrom(src =>
        src.Voucher != null ? src.Voucher.Code : null))
    .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src =>
        src.Price - src.FinalPrice))
    .ForMember(dest => dest.HasFeedback, opt => opt.MapFrom(src =>
        src.Feedback != null))
    .ForMember(dest => dest.ItineraryId, opt => opt.MapFrom(src =>
        (Guid?)src.ItinerarySchedule.ItineraryId))
    .ForMember(dest => dest.GuideName, opt => opt.MapFrom(src =>
        src.ItinerarySchedule.Guide != null ? src.ItinerarySchedule.Guide.FullName ?? src.ItinerarySchedule.Guide.Username : null))
    .ForMember(dest => dest.GuidePhone, opt => opt.MapFrom(src =>
        src.ItinerarySchedule.Guide != null ? src.ItinerarySchedule.Guide.Phone : null));
        }

    }
}
