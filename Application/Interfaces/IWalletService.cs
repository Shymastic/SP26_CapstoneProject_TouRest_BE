using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Payout;
using TouRest.Application.DTOs.Wallet;

namespace TouRest.Application.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDTO> GetWalletAsync(Guid userId);
        Task<List<SavedBankDTO>> GetSavedBanksAsync(Guid userId);
        Task RequestPayoutAsync(Guid userId, PayoutRequestDTO request);
        Task<List<PayoutDTO>> GetPendingPayoutsAsync();
        Task ApprovePayoutAsync(Guid payoutId, Guid adminId, string? adminNote = null);
        Task CompletePayoutAsync(Guid payoutId, Guid adminId, string transferReference);
        Task RejectPayoutAsync(Guid payoutId, string reason);
    }
}
