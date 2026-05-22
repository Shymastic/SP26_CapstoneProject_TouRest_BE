using TouRest.Domain.DTOs;

namespace TouRest.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardStatsDto> GetStatsAsync();
        Task<AdminTrendDto> GetTrendAsync(int year);
        Task<List<PendingApprovalDto>> GetPendingApprovalsAsync();
        Task<List<TopAgencyDto>> GetTopAgenciesAsync(int limit);
    }
}