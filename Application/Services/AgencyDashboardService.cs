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

    public class AgencyDashboardService : IAgencyDashboardService
    {
        private readonly IAgencyDashboardRepository _dashboardRepository;

        public AgencyDashboardService(IAgencyDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<AgencyDashboardStatsDTO> GetStatsAsync(Guid agencyId)
            => await _dashboardRepository.GetStatsAsync(agencyId);

        public async Task<List<UpcomingScheduleDTO>> GetUpcomingSchedulesAsync(Guid agencyId)
            => await _dashboardRepository.GetUpcomingSchedulesAsync(agencyId);

        public async Task<List<RecentBookingDTO>> GetRecentBookingsAsync(Guid agencyId)
            => await _dashboardRepository.GetRecentBookingsAsync(agencyId);

        public async Task<List<GuideWorkloadDTO>> GetGuideWorkloadAsync(Guid agencyId)
            => await _dashboardRepository.GetGuideWorkloadAsync(agencyId);
    }
}
