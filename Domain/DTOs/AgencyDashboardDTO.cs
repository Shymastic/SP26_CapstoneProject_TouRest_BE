using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Domain.DTOs
{
    public class AgencyDashboardStatsDTO
    {
        public int ActiveTours { get; set; }
        public int SchedulesToday { get; set; }
        public int MonthlyBookings { get; set; }
        public long MonthlyRevenue { get; set; }
        public int ActiveToursChangeThisMonth { get; set; }
        public int SchedulesTodayConfirmed { get; set; }
        public int SchedulesTodayPending { get; set; }
        public int MonthlyBookingsChangeVsLastMonth { get; set; }
        public decimal MonthlyRevenueChangePercent { get; set; }
    }

    public class UpcomingScheduleDTO
    {
        public Guid Id { get; set; }
        public string ItineraryName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public string? TourGuideName { get; set; }
        public int Pax { get; set; }
        public string Status { get; set; } = null!;
    }

    public class RecentBookingDTO
    {
        public string BookingCode { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string TourName { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; } = null!;
    }

    public class GuideWorkloadDTO
    {
        public Guid GuideId { get; set; }
        public string GuideName { get; set; } = null!;
        public int ActiveTours { get; set; }
        public int CompletedTotal { get; set; }
    }
}
