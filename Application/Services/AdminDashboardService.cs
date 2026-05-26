using TouRest.Application.Interfaces;
using TouRest.Domain.DTOs;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _repository;

        public AdminDashboardService(IAdminDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminDashboardStatsDto> GetStatsAsync()
        {
            return await _repository.GetStatsAsync();
        }

        public async Task<AdminTrendDto> GetTrendAsync(int year)
        {
            return await _repository.GetTrendAsync(year);
        }

        public async Task<List<PendingApprovalDto>> GetPendingApprovalsAsync()
        {
            return await _repository.GetPendingApprovalsAsync();
        }

        public async Task<List<TopAgencyDto>> GetTopAgenciesAsync(int limit)
        {
            return await _repository.GetTopAgenciesAsync(limit);
        }
    }
}