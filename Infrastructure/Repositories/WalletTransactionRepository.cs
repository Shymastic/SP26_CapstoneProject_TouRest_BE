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
    public class WalletTransactionRepository : BaseRepository<WalletTransaction>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(AppDbContext context) : base(context) { }
        public async Task<List<WalletTransaction>> GetByWalletIdAsync(Guid walletId)
    => await _context.WalletTransactions
        .Where(t => t.WalletId == walletId)
        .OrderByDescending(t => t.CreatedAt)
        .AsNoTracking()
        .ToListAsync();
    }
}
