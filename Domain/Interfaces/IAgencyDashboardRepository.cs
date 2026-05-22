using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.DTOs;

namespace TouRest.Domain.Interfaces
{
    public interface IAgencyDashboardRepository
    {
        Task<AgencyDashboardStatsDTO> GetStatsAsync(Guid agencyId);
        Task<List<UpcomingScheduleDTO>> GetUpcomingSchedulesAsync(Guid agencyId);
        Task<List<RecentBookingDTO>> GetRecentBookingsAsync(Guid agencyId);
        Task<List<GuideWorkloadDTO>> GetGuideWorkloadAsync(Guid agencyId);
    }
}
