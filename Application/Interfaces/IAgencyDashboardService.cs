using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.DTOs;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyDashboardService
    {
        Task<AgencyDashboardStatsDTO> GetStatsAsync(Guid agencyId);
        Task<List<UpcomingScheduleDTO>> GetUpcomingSchedulesAsync(Guid agencyId);
        Task<List<RecentBookingDTO>> GetRecentBookingsAsync(Guid agencyId);
        Task<List<GuideWorkloadDTO>> GetGuideWorkloadAsync(Guid agencyId);
    }

}
