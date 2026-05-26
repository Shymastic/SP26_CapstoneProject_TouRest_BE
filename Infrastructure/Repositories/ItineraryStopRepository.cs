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
    public class ItineraryStopRepository : BaseRepository<ItineraryStop>, IItineraryStopRepository
    {
        public ItineraryStopRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<ItineraryStop>> GetByItineraryIdAsync(Guid itineraryId)
        {
            return await _context.ItineraryStops
                .Where(s => s.ItineraryId == itineraryId).Include(s => s.Itinerary)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ItineraryStop>> GetWithActivitiesByItineraryIdAsync(Guid itineraryId)
        {
            return await _context.ItineraryStops
                .Where(s => s.ItineraryId == itineraryId)
                .Include(s => s.Vehicle)
                .Include(s => s.Activities.OrderBy(a => a.ActivityOrder))
                    .ThenInclude(a => a.Service)
                .OrderBy(s => s.StopOrder)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ItineraryStop>> GetWithProviderAndActivitiesByItineraryIdAsync(Guid itineraryId)
        {
            return await _context.ItineraryStops
                .Where(s => s.ItineraryId == itineraryId && s.ProviderId != null)
                .Include(s => s.Provider)
                .Include(s => s.Activities.OrderBy(a => a.ActivityOrder))
                    .ThenInclude(a => a.Service)
                .OrderBy(s => s.StopOrder)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ItineraryStop?> GetItineraryStop(Guid id)
        {
            return await _context.ItineraryStops
                .Include(s => s.Itinerary)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateRangeAsync(List<ItineraryStop> ordered)
        {
            _context.UpdateRange(ordered);
             await _context.SaveChangesAsync();
        }
    }
}
