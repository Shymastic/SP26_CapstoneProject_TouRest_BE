using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.DTOs;

namespace TouRest.Application.Interfaces
{
    public interface IProviderDashboardService
    {
        Task<ProviderDashboardStatsDto> GetStatsAsync(Guid providerId);
        Task<ProviderJobsTrendDto> GetJobTrendAsync(Guid providerId, int year);
        Task<List<PendingRequestDto>> GetPendingRequestsAsync(Guid providerId);
        Task<List<ActivePackageDto>> GetActivePackagesAsync(Guid providerId);
        Task<List<ProviderTopAgencyDto>> GetTopAgenciesAsync(Guid providerId);
    }
}
