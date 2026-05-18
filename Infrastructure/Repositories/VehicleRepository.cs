using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context) { }

        public async Task<List<Vehicle>> GetByAgencyIdAsync(Guid agencyId)
        {
            return await _context.Vehicles
                .Where(v => v.AgencyId == agencyId)
                .OrderBy(v => v.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
