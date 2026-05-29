using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IWalletRepository : IBaseRepository<Wallet>
    {
        Task<Wallet?> GetByUserIdAsync(Guid userId);
        Task<Wallet?> GetByAgencyIdAsync(Guid agencyId);
        Task<Wallet?> GetByProviderIdAsync(Guid providerId);
        Task<List<WalletTransaction>> GetTransactionsByWalletIdAsync(Guid walletId);
        Task<Wallet?> GetByOwnerAsync(Guid userId);
    }
}
