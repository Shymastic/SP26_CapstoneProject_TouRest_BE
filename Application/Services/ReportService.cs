using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text.Json;
using TouRest.Application.DTOs.Report;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<List<ReportDTO>> GetReports(ReportSearch search)
        {
            var list = await _reportRepository.GetReports(search);
            return _mapper.Map<List<ReportDTO>>(list);
        }

        public async Task<ReportDTO> GetReport(Guid id)
        {
            var report = await _reportRepository.GetReport(id);
            return _mapper.Map<ReportDTO>(report);
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
