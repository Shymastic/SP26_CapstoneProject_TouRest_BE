using TouRest.Domain.DTOs;

namespace TouRest.Domain.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<AdminDashboardStatsDto> GetStatsAsync();
        Task<AdminTrendDto> GetTrendAsync(int year);
        Task<List<PendingApprovalDto>> GetPendingApprovalsAsync();
        Task<List<TopAgencyDto>> GetTopAgenciesAsync(int limit);
    }
}