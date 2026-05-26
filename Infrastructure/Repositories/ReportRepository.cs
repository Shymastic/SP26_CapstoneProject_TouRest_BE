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
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        public ReportRepository(AppDbContext context) : base(context)
        {
        }
/*        public async Task<List<Report>> GetReportsByItemIdAsync(Guid itemId, ReportItemType type)
        {
            return await _context.Reports.AsNoTracking().Where(r => r.ItemId == itemId && r.ItemType == type).ToListAsync();
        }*/
        public async Task<Report?> GetReport(Guid id)
        {
            return await _context.Reports.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
        }
/*         public async Task<List<Report>> GetReports()
        {
            return await _context.Reports.AsNoTracking().ToListAsync();
        }*/
         public async Task<List<Report>> GetReports(ReportSearch search)
        {
            var query = _context.Reports.AsNoTracking().Include(r => r.User).AsQueryable();
            if (!string.IsNullOrEmpty(search.UserName)){
                query = query.Where(r => r.User.Username.Contains(search.UserName));
            }
            if(!string.IsNullOrEmpty(search.Title))
            {
                query = query.Where(r => r.Title.Contains(search.Title));
            }
            if(search.ItemType.HasValue)
            {
                query = query.Where(r => r.ItemType == search.ItemType.Value);
                if(search.ItemId.HasValue)
                {
                    query = query.Where(r => r.ItemId == search.ItemId.Value);
                }
            }
            if(search.Status.HasValue)
            {
                query = query.Where(r => r.Status == search.Status.Value);
            }
            return await query.ToListAsync();
        }
    }
}
