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
    public class PackageRepository : BaseRepository<Package>, IPackageRepository
    {
        public PackageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Package?> GetByCodeAsync(string code)
        {
            return await _context.Packages
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<List<Package>> GetByProviderIdWithServicesAsync(Guid providerId)
        {
            return await _context.Packages
                .AsNoTracking()
                .Include(p => p.PackageServices.OrderBy(ps => ps.SortOrder))
                    .ThenInclude(ps => ps.Service)
                .Where(p => p.PackageServices.Any(ps => ps.Service.ProviderId == providerId))
                .ToListAsync();
        }

        public async Task<Package?> GetByIdWithServicesAsync(Guid id)
        {
            return await _context.Packages
                .AsNoTracking()
                .Include(p => p.PackageServices.OrderBy(ps => ps.SortOrder))
                    .ThenInclude(ps => ps.Service)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
