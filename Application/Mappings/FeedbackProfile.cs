using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Feedback, FeedbackDTO>()
                .ForMember(dest => dest.CreateAt,      opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Username,      opt => opt.MapFrom(src =>
                    src.BookingItinerary != null && src.BookingItinerary.Booking != null && src.BookingItinerary.Booking.User != null
                        ? src.BookingItinerary.Booking.User.Username : null))
                .ForMember(dest => dest.UserAvatar,    opt => opt.MapFrom(src =>
                    src.BookingItinerary != null && src.BookingItinerary.Booking != null && src.BookingItinerary.Booking.User != null
                        ? src.BookingItinerary.Booking.User.UserAvatar : null))
                .ForMember(dest => dest.ItineraryId,   opt => opt.MapFrom(src =>
                    src.BookingItinerary != null && src.BookingItinerary.ItinerarySchedule != null
                        ? src.BookingItinerary.ItinerarySchedule.ItineraryId : Guid.Empty))
                .ForMember(dest => dest.ItineraryName, opt => opt.MapFrom(src =>
                    src.BookingItinerary != null && src.BookingItinerary.ItinerarySchedule != null && src.BookingItinerary.ItinerarySchedule.Itinerary != null
                        ? src.BookingItinerary.ItinerarySchedule.Itinerary.Name : null));
            // For creating new feedback, we don't want to include Id, CreatedAt, UpdatedAt, and BookingItinerary navigation property
            CreateMap<FeedbackCreateRequest, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BookingItinerary, opt => opt.Ignore());
            // For updating feedback, we want to ignore Id, CreatedAt, and BookingItinerary navigation property
            CreateMap<FeedbackUpdateRequest, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BookingItinerary, opt => opt.Ignore())
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));
            // For feedback summary, we only want to include Id, Rating, Title, and CreatedAt
            CreateMap<Feedback, FeedbackSummaryDTO>();
        }
    }
}
