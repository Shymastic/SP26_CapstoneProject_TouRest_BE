using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Image;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.DTOs.ItineraryActivity;
using TouRest.Application.DTOs.ItineraryStop;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Mappings
{
    public class ItineraryProfile : Profile
    {
        public ItineraryProfile()
        {
            CreateMap<Image, ImageDTO>();
            // Map Itinerary entity to ItineraryDTO for responses
            CreateMap<Itinerary, ItineraryDTO>()
                .ForMember(dest => dest.StopCount, opt => opt.MapFrom(src => src.Stops.Count))
                .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Agency != null ? src.Agency.Name : null))
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            // Map ItinerarySchedule entity to ItineraryScheduleDTO (GuideName is resolved manually)
            CreateMap<ItinerarySchedule, ItineraryScheduleDTO>()
                .ForMember(dest => dest.GuideName, opt => opt.MapFrom(src => src.Guide != null ? (src.Guide.FullName ?? src.Guide.Username) : null));
            //Map ItineraryCreateDTO to Itinerary entity for creating new itineraries
            CreateMap<ItineraryCreateRequest, Itinerary>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.ItineraryStatus.Draft))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.AgencyId, opt => opt.Ignore()); // AgencyId will be set
            //Map ItineraryUpdateDTO to Itinerary entity for updating existing itineraries
            CreateMap<ItineraryUpdateRequest, Itinerary>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            //Map ItineraryActivity entity to ItineraryActivityDTO for responses
            CreateMap<ItineraryActivity, ItineraryActivityDTO>();
            //Map ItineraryActivityCreateDTO to ItineraryActivity entity for creating new itinerary activities
            CreateMap<ItineraryActivityCreateRequest, ItineraryActivity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            //Map ItineraryActivityUpdateDTO to ItineraryActivity entity for updating existing itinerary activities
            CreateMap<ItineraryActivityUpdateRequest, ItineraryActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id should not be updated
            //Map ItineraryStop entity to ItineraryStopDTO for responses
            CreateMap<ItineraryStop, ItineraryStopDTO>();
            //Map ItineraryStopCreateDTO to ItineraryStop entity for creating new itinerary stops
            CreateMap<ItineraryStopCreateRequest, ItineraryStop>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            //Map ItineraryStopUpdateDTO to ItineraryStop entity for updating existing itinerary stops
            CreateMap<ItineraryStopUpdateRequest, ItineraryStop>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id should not be updated
            //Map ItineraryActvity to ItineraryActivitySummaryDTO for summary responses
            CreateMap<ItineraryActivity, ItineraryActivitySummaryDTO>();
            //Map Itinerary to ItinerarySummaryDTO for summary responses
            CreateMap<Itinerary, ItinerarySummaryDTO>();
            //Map ItineraryStop to ItineraryStopSummaryDTO for summary responses
            CreateMap<ItineraryStop, ItineraryStopSummaryDTO>();
            // Map ItineraryActivity to StopActivityDTO
            CreateMap<ItineraryActivity, StopActivityDTO>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : null))
                .ForMember(dest => dest.ServiceDescription, opt => opt.MapFrom(src => src.Service != null ? src.Service.Description : null));
            // Map ItineraryStop to ItineraryStopWithActivitiesDTO
            CreateMap<ItineraryStop, ItineraryStopWithActivitiesDTO>()
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Name : null));
        }
    }
}
