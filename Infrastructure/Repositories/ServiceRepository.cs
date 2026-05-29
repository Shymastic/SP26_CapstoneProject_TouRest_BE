using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetByProviderIdAsync(Guid providerId)
        {
            return await _context.Services
                .AsNoTracking()
                .Include(x => x.Provider)
                .Where(s => s.ProviderId == providerId)
                .ToListAsync();
        }
        public override async Task<Service?> GetByIdAsync(Guid id)
        {
            return await _context.Services
                .AsNoTracking()
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public override async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services
                .AsNoTracking()
                .Include(x => x.Provider)
                .ToListAsync();
        }
    }
}
