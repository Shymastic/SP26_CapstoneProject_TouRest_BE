using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ItineraryScheduleRepository : BaseRepository<ItinerarySchedule>, IItineraryScheduleRepository
    {
        public ItineraryScheduleRepository(AppDbContext context) : base(context) { }

        public async Task<List<ItinerarySchedule>> GetByItineraryIdAsync(Guid itineraryId)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Guide)
                .Where(s => s.ItineraryId == itineraryId)
                .OrderBy(s => s.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ItinerarySchedule?> GetByIdWithGuideAsync(Guid id)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Guide)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<ItinerarySchedule?> GetScheduleWithDetails(Guid scheduleId)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Itinerary)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);
        }

        public async Task<List<ItinerarySchedule>> GetByAgencyIdAsync(Guid agencyId)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Itinerary)
                .Include(s => s.Guide)
                .Where(s => s.Itinerary.AgencyId == agencyId)
                .OrderBy(s => s.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ItinerarySchedule>> GetByGuideIdAsync(Guid guideId)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Itinerary)
                .Include(s => s.Guide)
                .Where(s => s.GuideId == guideId)
                .OrderBy(s => s.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ItinerarySchedule>> GetByProviderIdAsync(Guid providerId)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Itinerary).ThenInclude(i => i.Agency)
                .Include(s => s.Guide)
                .Where(s => s.Itinerary.Stops.Any(stop => stop.ProviderId == providerId))
                .OrderBy(s => s.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
