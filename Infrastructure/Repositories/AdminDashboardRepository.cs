using Microsoft.EntityFrameworkCore;
using TouRest.Domain.DTOs;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly AppDbContext _context;

        public AdminDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardStatsDto> GetStatsAsync()
        {
            var now = DateTime.UtcNow;
            var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

            var registeredAgencies = await _context.Set<Agency>().CountAsync();
            var newAgenciesThisMonth = await _context.Set<Agency>().CountAsync(a => a.CreatedAt >= firstDayThisMonth);

            var registeredProviders = await _context.Set<Provider>().CountAsync();
            var newProvidersThisMonth = await _context.Set<Provider>().CountAsync(p => p.CreatedAt >= firstDayThisMonth);

            var platformBookings = await _context.Set<BookingItinerary>().CountAsync();
            var newBookingsThisMonth = await _context.Set<BookingItinerary>().CountAsync(b => b.CreatedAt >= firstDayThisMonth);

            var revenueThisMonth = await _context.Set<BookingItinerary>()
                .Where(b => b.Status == BookingItineraryStatus.Completed && b.CreatedAt >= firstDayThisMonth)
                .SumAsync(b => b.FinalPrice);

            var revenueLastMonth = await _context.Set<BookingItinerary>()
                .Where(b => b.Status == BookingItineraryStatus.Completed && b.CreatedAt >= firstDayLastMonth && b.CreatedAt < firstDayThisMonth)
                .SumAsync(b => b.FinalPrice);

            double revenuePercentageChange = 0;
            if (revenueLastMonth > 0)
            {
                revenuePercentageChange = Math.Round(((double)(revenueThisMonth - revenueLastMonth) / revenueLastMonth) * 100, 1);
            }
            else if (revenueThisMonth > 0)
            {
                revenuePercentageChange = 100;
            }

            var pendingAgenciesCount = await _context.Set<Agency>().CountAsync(a => a.Status == AgencyStatus.Pending);
            var pendingProvidersCount = await _context.Set<Provider>().CountAsync(p => p.Status == ProviderStatus.Pending);

            return new AdminDashboardStatsDto
            {
                RegisteredAgencies = registeredAgencies,
                NewAgenciesThisMonth = newAgenciesThisMonth,
                RegisteredProviders = registeredProviders,
                NewProvidersThisMonth = newProvidersThisMonth,
                PlatformBookings = platformBookings,
                NewBookingsThisMonth = newBookingsThisMonth,
                PlatformRevenue = revenueThisMonth,
                RevenuePercentageChange = revenuePercentageChange,
                PendingReviewsCount = pendingAgenciesCount + pendingProvidersCount,
            };
        }

        public async Task<AdminTrendDto> GetTrendAsync(int year)
        {
            var yearlyBookings = await _context.Set<BookingItinerary>()
                .Where(b => b.CreatedAt.Year == year && b.Status == BookingItineraryStatus.Completed)
                .ToListAsync();

            var totalBookingsYtd = yearlyBookings.Count;

            var monthlyGroup = yearlyBookings
                .GroupBy(b => b.CreatedAt.Month)
                .ToDictionary(g => g.Key, g => new
                {
                    Count = g.Count(),
                    Revenue = g.Sum(b => b.FinalPrice)
                });

            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var trends = new List<MonthlyTrendDto>();

            for (int i = 1; i <= 12; i++)
            {
                monthlyGroup.TryGetValue(i, out var data);

                trends.Add(new MonthlyTrendDto
                {
                    Month = months[i - 1],
                    BookingsCount = data?.Count ?? 0,
                    Revenue = data?.Revenue ?? 0
                });
            }

            return new AdminTrendDto
            {
                Year = year,
                TotalBookingsYtd = totalBookingsYtd,
                MonthlyTrends = trends
            };
        }

        public async Task<List<PendingApprovalDto>> GetPendingApprovalsAsync()
        {
            var pendingAgencies = await _context.Set<Agency>()
                .Where(a => a.Status == AgencyStatus.Pending)
                .Select(a => new PendingApprovalDto
                {
                    RequestId = a.Id,
                    Name = a.Name,
                    ShortName = a.Name.Substring(0, Math.Min(2, a.Name.Length)).ToUpper(),
                    Type = "Agency",
                    SubmittedAt = a.CreatedAt
                })
                .ToListAsync();

            var pendingProviders = await _context.Set<Provider>()
                .Where(p => p.Status == ProviderStatus.Pending)
                .Select(p => new PendingApprovalDto
                {
                    RequestId = p.Id,
                    Name = p.Name,
                    ShortName = p.Name.Substring(0, Math.Min(2, p.Name.Length)).ToUpper(),
                    Type = "Provider",
                    SubmittedAt = p.CreatedAt
                })
                .ToListAsync();

            return pendingAgencies.Concat(pendingProviders)
                .OrderByDescending(r => r.SubmittedAt)
                .ToList();
        }

        public async Task<List<TopAgencyDto>> GetTopAgenciesAsync(int limit)
        {
            return await _context.Set<Agency>()
                .Select(a => new TopAgencyDto
                {
                    AgencyId = a.Id,
                    AgencyName = a.Name,
                    ToursCount = _context.Set<BookingItinerary>()
                        .Where(bi => bi.ItinerarySchedule.Itinerary.AgencyId == a.Id)
                        .Select(bi => bi.ItinerarySchedule.ItineraryId)
                        .Distinct()
                        .Count(),
                    BookingsCount = _context.Set<BookingItinerary>()
                        .Count(bi => bi.ItinerarySchedule.Itinerary.AgencyId == a.Id),
                    TotalRevenue = _context.Set<BookingItinerary>()
                        .Where(bi => bi.ItinerarySchedule.Itinerary.AgencyId == a.Id)
                        .Sum(bi => (long?)bi.FinalPrice) ?? 0,
                    Status = a.Status.ToString()
                })
                .OrderByDescending(a => a.TotalRevenue)
                .Take(limit)
                .ToListAsync();
        }
    }
}