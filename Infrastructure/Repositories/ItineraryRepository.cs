using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ItineraryRepository : BaseRepository<Itinerary>, IItineraryRepository
    {
        public ItineraryRepository(AppDbContext context) : base(context) { }

        private IQueryable<Itinerary> BuildFilterQuery(ItinerarySearch search)
        {
            var query = _context.Itineraries
                .Include(x => x.Agency)
                .Include(x => x.Stops)
                .AsNoTracking()
                .AsQueryable();

            if (search.AgencyId != null)
                query = query.Where(x => x.AgencyId == search.AgencyId);

            if (search.AgencyName != null)
                query = query.Where(x => x.Agency.Name == search.AgencyName);

            if (!string.IsNullOrWhiteSpace(search.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{search.Name}%"));

            if (search.LowPrice != null)
                query = query.Where(x => x.Price >= search.LowPrice);

            if (search.HighPrice != null)
                query = query.Where(x => x.Price <= search.HighPrice);

            if (search.LowDurationDay != null)
                query = query.Where(x => x.DurationDays >= search.LowDurationDay);

            if (search.HighDurationDay != null)
                query = query.Where(x => x.DurationDays <= search.HighDurationDay);

            if (!string.IsNullOrWhiteSpace(search.Status) &&
                Enum.TryParse<Domain.Enums.ItineraryStatus>(search.Status, ignoreCase: true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            if (search.VehicleType != null)
            {
                query = query.Where(x => x.Stops.Any(s => s.Vehicle.Type == search.VehicleType));
            }
            if (!string.IsNullOrWhiteSpace(search.Destination))
            {
                query = query.Where(x => x.Stops.Any(s => s.Address != null && s.Address.Contains(search.Destination)));
            } 
            return query;
        }

        public async Task<List<Itinerary>> GetItineraries(ItinerarySearch search)
        {
            var query = BuildFilterQuery(search).OrderByDescending(x => x.CreatedAt);

            if (search.Limit != null)
                return await query.Take(search.Limit.Value).ToListAsync();

            var skip = (search.Page - 1) * search.PageSize;
            return await query.Skip(skip).Take(search.PageSize).ToListAsync();
        }

        public async Task<int> CountItineraries(ItinerarySearch search)
            => await BuildFilterQuery(search).CountAsync();

        public async Task<List<Itinerary>> GetByAgencyIdAsync(Guid agencyId)
        {
            return await _context.Itineraries
                .Include(x => x.Stops)
                .Where(x => x.AgencyId == agencyId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Itinerary?> GetByIdAsync(Guid id)
        {
            return await _context.Itineraries
                .Include(x => x.Agency)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<int> CountActiveByAgencyIdAsync(Guid agencyId)
        {
            return await _context.Itineraries
                .CountAsync(i => i.AgencyId == agencyId && i.Status == ItineraryStatus.Active);
        }
    }
}
