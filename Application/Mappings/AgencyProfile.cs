using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class AgencyProfile : Profile
    {
        public AgencyProfile()
        {

            CreateMap<Agency, AgencyDTO>()
                .ForMember(dest =>dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<AgencyCreateRequestDTO, Agency>()
                .ForMember(dest => dest.StartTime, opt => opt.Ignore())
                .ForMember(dest => dest.EndTime, opt => opt.Ignore())
                .ForMember(dest => dest.CreateByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<AgencyUpdateRequestDTO, Agency>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            // Mapping for AgencyUserDTO
            CreateMap<AgencyUser, AgencyUserDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Agency.Name))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));


        }
    }
}
