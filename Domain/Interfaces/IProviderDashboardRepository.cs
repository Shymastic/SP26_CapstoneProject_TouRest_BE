using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.DTOs;

namespace TouRest.Domain.Interfaces
{
    public interface IProviderDashboardRepository
    {
        Task<ProviderDashboardStatsDto> GetStatsAsync(Guid providerId);
        Task<ProviderJobsTrendDto> GetJobsTrendAsync(Guid providerId, int year);
        Task<List<PendingRequestDto>> GetPendingRequestsAsync(Guid providerId);
    }
}
