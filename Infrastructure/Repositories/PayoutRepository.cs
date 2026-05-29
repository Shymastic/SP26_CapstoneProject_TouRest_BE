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
    public class PayoutRepository : BaseRepository<Payout>, IPayoutRepository
    {
        public PayoutRepository(AppDbContext appDbContext) : base(appDbContext) { }
        public async Task<List<Payout>> GetByWalletIdAsync(Guid walletId)
    => await _context.Payouts
        .Where(p => p.WalletId == walletId)
        .OrderByDescending(p => p.CreatedAt)
        .AsNoTracking()
        .ToListAsync();
        public async Task<List<Payout>> GetPendingAsync()
    => await _context.Payouts
        .Where(p => p.Status == PayoutStatus.Pending)
        .OrderBy(p => p.CreatedAt)
        .AsNoTracking()
        .ToListAsync();
    }
}
