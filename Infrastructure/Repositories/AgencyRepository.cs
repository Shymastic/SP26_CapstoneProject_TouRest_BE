using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class AgencyRepository : BaseRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository(AppDbContext context) : base(context) { }


        public async Task<Agency?> GetByContactEmailAsync(string contactEmail)
        {
            return await _context.Agencies
                .FirstOrDefaultAsync(a => a.ContactEmail == contactEmail);
        }
        public async Task<Agency?> GetMyAgency(Guid userId)
        {
            return await _context.AgencyUsers
                .Where(au => au.UserId == userId)
                .Select(au => au.Agency)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<Agency> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Agencies
                .Where(a => a.Status != AgencyStatus.Pending)
                .OrderByDescending(a => a.CreatedAt);
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
        public async Task<Agency?> GetAgencyByIdWithCreator(Guid agencyId)
        {
            return await _context.Agencies.Include(x=>x.User).AsNoTracking().FirstOrDefaultAsync(x=>x.Id == agencyId);
        }

    }
}
