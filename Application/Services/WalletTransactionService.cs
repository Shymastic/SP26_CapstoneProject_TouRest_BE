using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Wallet;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IWalletTransactionRepository _repo;
        private readonly IWalletRepository _walletRepository;
        public WalletTransactionService(IWalletTransactionRepository repo, IWalletRepository walletRepository)
        {
            _repo = repo;
            _walletRepository = walletRepository;
        }
        public async Task<List<WalletTransactionDTO>> GetTransactionsAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByOwnerAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found");
            var transactions = await _repo.GetByWalletIdAsync(wallet.Id);
            return transactions.Select(t => new WalletTransactionDTO
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Reason = t.Reason,
                ReferenceId = t.ReferenceId,
                Note = t.Note,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}
