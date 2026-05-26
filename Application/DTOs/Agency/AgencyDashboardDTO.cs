using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyDashboardDTO
    {
        public int TotalItineraries { get; set; }
        public int ActiveItineraries { get; set; }
        public int TotalSchedules { get; set; }
        public int ActiveSchedules { get; set; }
        public AgencyTodayScheduleDTO AgencyTodaySchedule { get; set; } = new AgencyTodayScheduleDTO();
        public int MonthlyBookings { get; set; }
        public int DailyBookings { get; set; }
        public int YearlyBookings { get; set; }
        public int TotalRevenue { get; set; }
        public int MonthlyRevenue { get; set; }
        
    }
}
