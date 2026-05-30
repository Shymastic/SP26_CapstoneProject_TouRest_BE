using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.DTOs;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class AgencyDashboardRepository : IAgencyDashboardRepository
    {
        private readonly AppDbContext _context;

        public AgencyDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AgencyDashboardStatsDTO> GetStatsAsync(Guid agencyId)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var today = now.Date;

            var activeTours = await _context.Itineraries
                .CountAsync(i => i.AgencyId == agencyId && i.Status == ItineraryStatus.Active);

            var activeToursLastMonth = await _context.Itineraries
                .CountAsync(i => i.AgencyId == agencyId
                    && i.Status == ItineraryStatus.Active
                    && i.CreatedAt < startOfMonth);

            var schedulesToday = await _context.ItinerarySchedules
                .Include(s => s.Itinerary)
                .Where(s => s.Itinerary.AgencyId == agencyId
                    && s.StartTime.Date == today)
                .ToListAsync();

            var monthlyBookings = await _context.BookingItineraries
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Itinerary)
                .Where(bi => bi.ItinerarySchedule.Itinerary.AgencyId == agencyId
                    && bi.CreatedAt >= startOfMonth)
                .CountAsync();

            var lastMonthBookings = await _context.BookingItineraries
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Itinerary)
                .Where(bi => bi.ItinerarySchedule.Itinerary.AgencyId == agencyId
                    && bi.CreatedAt >= startOfLastMonth
                    && bi.CreatedAt < startOfMonth)
                .CountAsync();

            var monthlyRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Paid
                    && p.PaidAt >= startOfMonth
                    && p.Booking.BookingItineraries
                        .Any(bi => bi.ItinerarySchedule.Itinerary.AgencyId == agencyId))
                .SumAsync(p => p.FinalAmount);

            var lastMonthRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Paid
                    && p.PaidAt >= startOfLastMonth
                    && p.PaidAt < startOfMonth
                    && p.Booking.BookingItineraries
                        .Any(bi => bi.ItinerarySchedule.Itinerary.AgencyId == agencyId))
                .SumAsync(p => p.FinalAmount);

            var revenueChangePercent = lastMonthRevenue == 0 ? 0 :
                decimal.Round((monthlyRevenue - lastMonthRevenue) / lastMonthRevenue * 100, 1);

            return new AgencyDashboardStatsDTO
            {
                ActiveTours = activeTours,
                ActiveToursChangeThisMonth = activeTours - activeToursLastMonth,
                SchedulesToday = schedulesToday.Count,
                SchedulesTodayConfirmed = schedulesToday.Count(s => s.Status == ItineraryScheduleStatus.Confirmed),
                SchedulesTodayPending = schedulesToday.Count(s => s.Status == ItineraryScheduleStatus.Pending),
                MonthlyBookings = monthlyBookings,
                MonthlyBookingsChangeVsLastMonth = monthlyBookings - lastMonthBookings,
                MonthlyRevenue = (long)monthlyRevenue,
                MonthlyRevenueChangePercent = revenueChangePercent
            };
        }

        public async Task<List<UpcomingScheduleDTO>> GetUpcomingSchedulesAsync(Guid agencyId)
        {
            var now = DateTime.UtcNow;
            return await _context.ItinerarySchedules
                .Include(s => s.Itinerary)
                .Include(s => s.Guide)
                .Where(s => s.Itinerary.AgencyId == agencyId && s.StartTime >= now)
                .OrderBy(s => s.StartTime)
                .Take(10)
                .Select(s => new UpcomingScheduleDTO
                {
                    Id = s.Id,
                    ItineraryName = s.Itinerary.Name,
                    StartTime = s.StartTime,
                    TourGuideName = s.Guide != null ? s.Guide.FullName : null,
                    Pax = s.Itinerary.MaxCapacity - s.SpotLeft,
                })
                .ToListAsync();
        }

        public async Task<List<RecentBookingDTO>> GetRecentBookingsAsync(Guid agencyId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItineraries)
                    .ThenInclude(bi => bi.ItinerarySchedule)
                        .ThenInclude(s => s.Itinerary)
                .Where(b => b.BookingItineraries
                    .Any(bi => bi.ItinerarySchedule.Itinerary.AgencyId == agencyId))
                .OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .Select(b => new RecentBookingDTO
                {
                    BookingCode = b.Code,
                    CustomerName = b.User.FullName ?? b.User.Username,
                    TourName = b.BookingItineraries
                        .First().ItinerarySchedule.Itinerary.Name,
                    BookingDate = b.CreatedAt,
                    Amount = b.TotalAmount,
                    Status = b.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<List<GuideWorkloadDTO>> GetGuideWorkloadAsync(Guid agencyId)
        {
            return await _context.AgencyUsers
                .Include(au => au.User)
                .Where(au => au.AgencyId == agencyId && au.Role == AgencyUserRole.TourGuide)
                .Select(au => new GuideWorkloadDTO
                {
                    GuideId = au.UserId,
                    GuideName = au.User.FullName ?? au.User.Username,
                    ActiveTours = _context.ItinerarySchedules
                        .Count(s => s.GuideId == au.UserId
                            && s.StartTime >= DateTime.UtcNow
                            && s.Status == ItineraryScheduleStatus.Confirmed),
                    CompletedTotal = _context.ItinerarySchedules
                        .Count(s => s.GuideId == au.UserId
                            && s.Status == ItineraryScheduleStatus.Completed)
                })
                .ToListAsync();
        }
    }
}
