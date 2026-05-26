using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Domain.DTOs
{
    public class ProviderDashboardStatsDto
    {
        public int ActiveServices { get; set; }
        public int ActiveServicesChangeThisMonth { get; set; }
        public int ActivePackages { get; set; }
        public int AgenciesSubscribedCount { get; set; }
        public int PendingRequestsCount { get; set; }
        public int NewPendingRequestsToday { get; set; }
        public long MonthlyRevenue { get; set; }
        public int RevenuePercentageChange { get; set; }
    }

    public class ProviderJobsTrendDto
    {
        public int Year { get; set; }
        public List<MonthlyJobTrendDto> MonthlyTrends { get; set; } = new();
    }

    public class MonthlyJobTrendDto
    {
        public string Month { get; set; } = null!;
        public int JobsCount { get; set; }
        public string Status { get; set; } = null!;
    }

    public class PendingRequestDto
    {
        public Guid RequestId { get; set; }
        public string AgencyName { get; set; } = null!;
        public string AgencyShortName { get; set; } = null!;
        public string PackageOrServiceName { get; set; } = null!;
        public int Pax { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsUrgent { get; set; }
    }
}
