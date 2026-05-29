using Microsoft.EntityFrameworkCore;
using TouRest.Domain.DTOs;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ProviderDashboardRepository : IProviderDashboardRepository
    {
        private readonly AppDbContext _context;

        public ProviderDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderDashboardStatsDto> GetStatsAsync(Guid providerId)
        {
            var now = DateTime.UtcNow;
            var startOfToday = now.Date;
            var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

            var activeServices = await _context.Set<Service>()
                .CountAsync(s => s.ProviderId == providerId && s.Status == ServiceStatus.Active);

            var servicesCreatedThisMonth = await _context.Set<Service>()
                .CountAsync(s => s.ProviderId == providerId &&
                                 s.Status == ServiceStatus.Active &&
                                 s.CreatedAt >= firstDayThisMonth);

            var activePackages = await _context.Set<PackageService>()
                .Where(ps => ps.Service.ProviderId == providerId && ps.Package.Status == PackageStatus.Active)
                .Select(ps => ps.PackageId)
                .Distinct()
                .CountAsync();
            var subscribedAgenciesCount = await _context.Set<Itinerary>()
                .Where(i => i.Stops.Any(s => s.Activities.Any(a => a.Service!.ProviderId == providerId)))
                .Select(i => i.AgencyId)
                .Distinct()
                .CountAsync();

            var pendingRequestsQuery = _context.Set<BookingItinerary>()
                .Where(bi => bi.Status == BookingItineraryStatus.Pending &&
                             bi.ItinerarySchedule.Itinerary.Stops
                                .Any(s => s.Activities.Any(a => a.Service!.ProviderId == providerId)));

            var pendingRequestsCount = await pendingRequestsQuery.CountAsync();

            var newPendingRequestsToday = await pendingRequestsQuery
                .CountAsync(bi => bi.CreatedAt >= startOfToday);
            var revenueThisMonth = await _context.Set<BookingItinerary>()
                .Where(bi => bi.Status == BookingItineraryStatus.Completed &&
                             bi.ItinerarySchedule.StartTime >= firstDayThisMonth &&
                             bi.ItinerarySchedule.Itinerary.Stops
                                .Any(s => s.Activities.Any(a => a.Service!.ProviderId == providerId)))
                .SumAsync(bi => bi.FinalPrice);

            var revenueLastMonth = await _context.Set<BookingItinerary>()
                .Where(bi => bi.Status == BookingItineraryStatus.Completed &&
                             bi.ItinerarySchedule.StartTime >= firstDayLastMonth &&
                             bi.ItinerarySchedule.StartTime < firstDayThisMonth &&
                             bi.ItinerarySchedule.Itinerary.Stops
                                .Any(s => s.Activities.Any(a => a.Service!.ProviderId == providerId)))
                .SumAsync(bi => bi.FinalPrice);

            int revenuePercentageChange = 0;
            if (revenueLastMonth > 0)
            {
                revenuePercentageChange = (int)(((double)(revenueThisMonth - revenueLastMonth) / revenueLastMonth) * 100);
            }
            else if (revenueThisMonth > 0)
            {
                revenuePercentageChange = 100;
            }

            return new ProviderDashboardStatsDto
            {
                ActiveServices = activeServices,
                ActiveServicesChangeThisMonth = servicesCreatedThisMonth,
                ActivePackages = activePackages,
                AgenciesSubscribedCount = subscribedAgenciesCount,
                PendingRequestsCount = pendingRequestsCount,
                NewPendingRequestsToday = newPendingRequestsToday,
                MonthlyRevenue = revenueThisMonth,
                RevenuePercentageChange = revenuePercentageChange
            };
        }

        public async Task<ProviderJobsTrendDto> GetJobsTrendAsync(Guid providerId, int year)
        {
            var now = DateTime.UtcNow;

            var monthlyCounts = await _context.Set<BookingItinerary>()
                .Where(bi => bi.ItinerarySchedule.StartTime.Year == year &&
                             bi.ItinerarySchedule.Itinerary.Stops
                                .Any(s => s.Activities.Any(a => a.Service!.ProviderId == providerId)))
                .GroupBy(bi => bi.ItinerarySchedule.StartTime.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var trends = new List<MonthlyJobTrendDto>();

            for (int i = 1; i <= 12; i++)
            {
                var record = monthlyCounts.FirstOrDefault(m => m.Month == i);
                string status = i == now.Month ? "Current month" : i < now.Month ? "Past months" : "Upcoming";

                trends.Add(new MonthlyJobTrendDto
                {
                    Month = months[i - 1],
                    JobsCount = record?.Count ?? 0,
                    Status = status
                });
            }

            return new ProviderJobsTrendDto
            {
                Year = year,
                MonthlyTrends = trends
            };
        }

        public async Task<List<PendingRequestDto>> GetPendingRequestsAsync(Guid providerId)
        {
            var now = DateTime.UtcNow;

            return await _context.Set<BookingItinerary>()
                .Where(bi => bi.Status == BookingItineraryStatus.Pending &&
                             bi.ItinerarySchedule.Itinerary.Stops
                                .Any(s => s.Activities.Any(a => a.Service!.ProviderId == providerId)))
                .Select(bi => new PendingRequestDto
                {
                    RequestId = bi.Id,
                    AgencyName = bi.ItinerarySchedule.Itinerary.Agency.Name,
                    AgencyShortName = bi.ItinerarySchedule.Itinerary.Agency.Name.Substring(0, Math.Min(2, bi.ItinerarySchedule.Itinerary.Agency.Name.Length)).ToUpper(),
                    PackageOrServiceName = bi.ItinerarySchedule.Itinerary.Stops
                        .SelectMany(s => s.Activities)
                        .Where(a => a.Service != null && a.Service.ProviderId == providerId)
                        .Select(a => a.Service!.Name)
                        .FirstOrDefault() ?? "Unknown Service",
                    Pax = bi.NumberOfGuests,
                    ScheduledTime = bi.ItinerarySchedule.StartTime,
                    IsUrgent = bi.ItinerarySchedule.StartTime <= now.AddHours(24)
                })
                .ToListAsync();
        }
    }
}