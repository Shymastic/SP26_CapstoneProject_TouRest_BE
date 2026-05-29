using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ItineraryTrackingRepository : BaseRepository<ItineraryTracking>, IItineraryTrackingRepository
    {
        public ItineraryTrackingRepository(AppDbContext context) : base(context) { }

        public async Task<List<ItineraryTracking>> GetByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.ItineraryTrackings
                .AsNoTracking()
                .Where(t => t.ItineraryScheduleId == scheduleId)
                .ToListAsync();
        }

        public async Task<ItineraryTracking?> FindAsync(Guid scheduleId, Guid trackingId, ItineraryTrackingType type)
        {
            return await _context.ItineraryTrackings
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.ItineraryScheduleId == scheduleId
                                       && t.TrackingId == trackingId
                                       && t.Type == type);
        }
    }
}
