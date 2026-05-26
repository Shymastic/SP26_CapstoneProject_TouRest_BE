using System;
using System.Collections.Generic;

namespace TouRest.Domain.DTOs
{

    public class AdminDashboardStatsDto
    {
        public int RegisteredAgencies { get; set; }
        public int NewAgenciesThisMonth { get; set; }

        public int RegisteredProviders { get; set; }
        public int NewProvidersThisMonth { get; set; }

        public int PlatformBookings { get; set; }
        public int NewBookingsThisMonth { get; set; }

        public long PlatformRevenue { get; set; }
        public double RevenuePercentageChange { get; set; }
        public int PendingReviewsCount { get; set; }
        //// Platform Health (Mocked or DB-driven)
        //public double SystemUptimePercentage { get; set; } // e.g., 99.8
        //public int ApiResponseTimeMs { get; set; } // e.g., 142
        // // e.g., 4
        //public double FailedPaymentsPercentage { get; set; } // e.g., 0.3
    }

    // 2. GET /admin/bookings/trend
    public class AdminTrendDto
    {
        public int Year { get; set; }
        public int TotalBookingsYtd { get; set; }
        public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new();
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } = null!;
        public int BookingsCount { get; set; }
        public long Revenue { get; set; }
    }

    public class PendingApprovalDto
    {
        public Guid RequestId { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public string Type { get; set; } = null!; 
        public DateTime SubmittedAt { get; set; }
    }

    public class TopAgencyDto
    {
        public Guid AgencyId { get; set; }
        public string AgencyName { get; set; } = null!;
        public int ToursCount { get; set; }
        public int BookingsCount { get; set; }
        public long TotalRevenue { get; set; }
        public string Status { get; set; } = null!;
    }
}