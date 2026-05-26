using AutoMapper;
using System.Collections.Generic;
using System.Text.Json;
using TouRest.Application.DTOs.Report;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Report, ReportDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ImageUrls)
                        ? null
                        : JsonSerializer.Deserialize<List<string>>(src.ImageUrls, (JsonSerializerOptions?)null)))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
                .ForMember(dest => dest.ItemName, opt => opt.Ignore());

            // ReportService.AddReport maps request → entity before saving.
            // ImageUrls is a JSON string in the entity; the service serializes it manually after mapping.
            CreateMap<ReportCreateRequest, Report>()
                .ForMember(dest => dest.Id,         opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,  opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt,  opt => opt.Ignore())
                .ForMember(dest => dest.User,       opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrls,  opt => opt.Ignore());
        }
    }
}
