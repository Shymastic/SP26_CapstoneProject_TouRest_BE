using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Service;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Application.Mappings
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            //Mapping configurations for Service
            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider != null ? src.Provider.Name : null));
            // For creating new service, we don't want to include Id, CreatedAt, UpdatedAt, and Provider navigation property
            CreateMap<ServiceCreateRequest, Service>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src=> Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ServiceStatus.Draft))
                .ForMember(dest => dest.Provider, opt => opt.Ignore());
            // For updating service, we want to ignore Id, CreatedAt, and Provider navigation property
            CreateMap<ServiceUpdateRequest, Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Provider, opt => opt.Ignore());
        }
    }
}
