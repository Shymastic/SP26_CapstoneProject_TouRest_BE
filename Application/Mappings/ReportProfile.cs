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
                        : JsonSerializer.Deserialize<List<string>>(src.ImageUrls, (JsonSerializerOptions?)null)));
        }
    }
}
