using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.DTOs;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ProviderDashboardService : IProviderDashboardService
    {
        private readonly IProviderDashboardRepository _dashboardRepository;

        public ProviderDashboardService(IProviderDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<ProviderDashboardStatsDto> GetStatsAsync(Guid providerId)
            => await _dashboardRepository.GetStatsAsync(providerId);

        public async Task<ProviderJobsTrendDto> GetJobTrendAsync(Guid providerId, int year)
            => await _dashboardRepository.GetJobsTrendAsync(providerId, year);

        public async Task<List<PendingRequestDto>> GetPendingRequestsAsync(Guid providerId)
            => await _dashboardRepository.GetPendingRequestsAsync(providerId);

        public async Task<List<ActivePackageDto>> GetActivePackagesAsync(Guid providerId)
            => await _dashboardRepository.GetActivePackagesAsync(providerId);

        public async Task<List<ProviderTopAgencyDto>> GetTopAgenciesAsync(Guid providerId)
            => await _dashboardRepository.GetTopAgenciesAsync(providerId);
    }
}
