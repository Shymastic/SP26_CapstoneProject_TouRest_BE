using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text.Json;
using TouRest.Application.DTOs.Report;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepository,
            IServiceRepository serviceRepository,
            IPackageRepository packageRepository,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _serviceRepository = serviceRepository;
            _packageRepository = packageRepository;
            _mapper = mapper;
        }

        public async Task<List<ReportDTO>> GetReports(ReportSearch search)
        {
            var list = await _reportRepository.GetReports(search);
            var dtos = _mapper.Map<List<ReportDTO>>(list);
            for (int i = 0; i < dtos.Count; i++)
                await ResolveItemName(dtos[i], list[i].ItemType, list[i].ItemId);
            return dtos;
        }

        public async Task<ReportDTO> GetReport(Guid id)
        {
            var report = await _reportRepository.GetReport(id);
            var dto = _mapper.Map<ReportDTO>(report);
            await ResolveItemName(dto, report.ItemType, report.ItemId);
            return dto;
        }

        private async Task ResolveItemName(ReportDTO dto, ReportItemType itemType, Guid itemId)
        {
            dto.ItemName = itemType switch
            {
                ReportItemType.Service => (await _serviceRepository.GetByIdAsync(itemId))?.Name,
                ReportItemType.Package => (await _packageRepository.GetByIdAsync(itemId))?.Name,
                _ => null
            };
        }

        public async Task<ReportDTO> AddReport(ReportCreateRequest create)
        {
            var report = _mapper.Map<Report>(create);

            if (create.ImageUrls?.Count > 0)
                report.ImageUrls = JsonSerializer.Serialize(create.ImageUrls);

            var result = await _reportRepository.CreateAsync(report);
            return _mapper.Map<ReportDTO>(result);
        }

        public async Task<bool> DeleteReport(Guid id)
        {
            return await _reportRepository.DeleteAsync(id);
        }

        public async Task<ReportDTO> UpdateReport(Guid id, ReportUpdateRequest update)
        {
            var report = _mapper.Map<Report>(update);
            report.Id = id;
            var result = await _reportRepository.UpdateAsync(report);
            return _mapper.Map<ReportDTO>(result);
        }
    }
}
