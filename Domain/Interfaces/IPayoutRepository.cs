using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IPayoutRepository : IBaseRepository<Payout>
    {
        Task<List<Payout>> GetByWalletIdAsync(Guid walletId);
        Task<List<Payout>> GetPendingAsync();
    }
}
