using AutoMapper;
using TouRest.Application.DTOs.Notification;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDTO>()
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType.ToString()));
        }
    }
}
