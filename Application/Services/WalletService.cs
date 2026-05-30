
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Payout;
using TouRest.Application.DTOs.Wallet;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAgencyUserRepository _agencyUserRepository;
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IPayoutRepository _payoutRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IItineraryScheduleRepository _scheduleRepository;
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        public WalletService(
            IWalletRepository walletRepository,
            IAgencyUserRepository agencyUserRepository,
            IProviderUserRepository providerUserRepository,
            IPayoutRepository payoutRepository,
            IWalletTransactionRepository walletTransactionRepository, 
            IItineraryScheduleRepository scheduleRepository, 
            IBookingItineraryRepository bookingItineraryRepository)
        {
            _walletRepository = walletRepository;
            _agencyUserRepository = agencyUserRepository;
            _providerUserRepository = providerUserRepository;
            _payoutRepository = payoutRepository;
            _scheduleRepository = scheduleRepository;
            _bookingItineraryRepository = bookingItineraryRepository;
            _walletTransactionRepository = walletTransactionRepository;
        }

        private async Task<(Wallet wallet, string ownerType)> ResolveWalletAsync(Guid userId)
        {
            // Check agency first
            var agencyUser = await _agencyUserRepository.GetAgencyUserByUserId(userId);
            if (agencyUser != null)
            {
                var wallet = await _walletRepository.GetByAgencyIdAsync(agencyUser.AgencyId)
                    ?? await CreateWalletAsync(agencyId: agencyUser.AgencyId);
                return (wallet, "Agency");
            }

            // Check provider
            var providerUser = await _providerUserRepository.GetByUserIdAsync(userId);
            if (providerUser != null)
            {
                var wallet = await _walletRepository.GetByProviderIdAsync(providerUser.ProviderId)
                    ?? await CreateWalletAsync(providerId: providerUser.ProviderId);
                return (wallet, "Provider");
            }

            // Fall back to user wallet (customer)
            var userWallet = await _walletRepository.GetByUserIdAsync(userId)
                ?? await CreateWalletAsync(userId: userId);
            return (userWallet, "User");
        }

        private async Task<Wallet> CreateWalletAsync(Guid? userId = null, Guid? agencyId = null, Guid? providerId = null)
        {
            var wallet = new Wallet
            {
                Id             = Guid.NewGuid(),
                UserId         = userId,
                AgencyId       = agencyId,
                ProviderId     = providerId,
                Balance        = 0,
                PendingBalance = 0,
                CreatedAt      = DateTime.UtcNow,
            };
            return await _walletRepository.CreateAsync(wallet);
        }

        public async Task<WalletDTO> GetWalletAsync(Guid userId)
        {
            var (wallet, ownerType) = await ResolveWalletAsync(userId);
            return new WalletDTO
            {
                Id = wallet.Id,
                Balance = wallet.Balance,
                PendingBalance = wallet.PendingBalance,
                OwnerType = ownerType
            };
        }

        public async Task<List<SavedBankDTO>> GetSavedBanksAsync(Guid userId)
        {
            var (wallet, _) = await ResolveWalletAsync(userId);
            var payouts = await _payoutRepository.GetByWalletIdAsync(wallet.Id);
            return payouts
                .GroupBy(p => p.BankAccount)
                .Select(g => new SavedBankDTO
                {
                    BankAccount = g.Key,
                    BankName = g.First().BankName,
                    AccountHolder = g.First().AccountHolder,
                    LastUsedAt = g.Max(p => p.CreatedAt)
                })
                .OrderByDescending(b => b.LastUsedAt)
                .ToList();
        }

        public async Task RequestPayoutAsync(Guid userId, PayoutRequestDTO request)
        {
            var (wallet, _) = await ResolveWalletAsync(userId);
            if (wallet.Balance - wallet.PendingBalance < request.Amount)
                throw new InvalidOperationException("Insufficient balance");
            wallet.PendingBalance += request.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);
            var payout = new Payout
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                Amount = request.Amount,
                BankAccount = request.BankAccount,
                BankName = request.BankName,
                AccountHolder = request.AccountHolder,
                Status = PayoutStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _payoutRepository.CreateAsync(payout);
            await RecordTransactionAsync(
             wallet.Id, request.Amount,
             WalletTransactionType.Debit,
             WalletTransactionReason.Payout,
             payout.Id,
             $"Payout request to {request.BankAccount} - {request.BankName}");
        }
        public async Task ApprovePayoutAsync(Guid payoutId, Guid adminId, string? adminNote = null)
        {
            var payout = await _payoutRepository.GetByIdAsync(payoutId);
            if (payout == null)
                throw new KeyNotFoundException("Payout not found");
            if (payout.Status != PayoutStatus.Pending)
                throw new InvalidOperationException("Payout is not pending");

            payout.Status = PayoutStatus.Processing;
            payout.AdminNote = adminNote ?? $"Approved by admin {adminId}";
            payout.UpdatedAt = DateTime.UtcNow;
            await _payoutRepository.UpdateAsync(payout);

            // Deduct balance
            var wallet = await _walletRepository.GetByIdAsync(payout.WalletId);
            wallet!.Balance -= payout.Amount;
            wallet.PendingBalance -= payout.Amount;

            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);
            await RecordTransactionAsync(
             payout.WalletId, payout.Amount,
             WalletTransactionType.Debit,
             WalletTransactionReason.Payout,
             payout.Id,
             $"Payout approved by admin {adminId}");
        }

        public async Task CompletePayoutAsync(Guid payoutId, Guid adminId, string transferReference)
        {
            var payout = await _payoutRepository.GetByIdAsync(payoutId);
            if (payout == null)
                throw new KeyNotFoundException("Payout not found");
            if (payout.Status != PayoutStatus.Processing)
                throw new InvalidOperationException("Payout is not in processing state");

            payout.Status = PayoutStatus.Completed;
            payout.PayOSTransferId = transferReference;
            payout.PaidAt = DateTime.UtcNow;
            payout.AdminNote = $"Completed by admin {adminId}";
            payout.UpdatedAt = DateTime.UtcNow;
            await _payoutRepository.UpdateAsync(payout);
    //        await RecordTransactionAsync(
    //payout.WalletId, payout.Amount,
    //WalletTransactionType.Debit,
    //WalletTransactionReason.Payout,
    //payout.Id,
    //$"Payout completed - ref: {transferReference}");
        }

        public async Task<List<PayoutDTO>> GetPendingPayoutsAsync()
        {
            var payouts = await _payoutRepository.GetPendingAsync();
            return payouts.Select(p => new PayoutDTO
            {
                Id = p.Id,
                WalletId = p.WalletId,
                Amount = p.Amount,
                BankAccount = p.BankAccount,
                BankName = p.BankName,
                AccountHolder = p.AccountHolder,
                Status = p.Status,
                AdminNote = p.AdminNote,
                PayOSTransferId = p.PayOSTransferId,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            }).ToList();
        }
        public async Task RejectPayoutAsync(Guid payoutId, string reason)
        {
            var payout = await _payoutRepository.GetByIdAsync(payoutId);
            if (payout == null)
                throw new KeyNotFoundException("Payout not found");
            if (payout.Status != PayoutStatus.Pending)
                throw new InvalidOperationException("Only pending payouts can be rejected");
            //refund balance
            var wallet = await _walletRepository.GetByIdAsync(payout.WalletId);
            wallet!.PendingBalance -= payout.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);

            payout.Status = PayoutStatus.Rejected;
            payout.AdminNote = reason;
            payout.UpdatedAt = DateTime.UtcNow;
            await _payoutRepository.UpdateAsync(payout);
            //await RecordTransactionAsync(
            //     payout.WalletId, payout.Amount,
            //     WalletTransactionType.Credit,
            //     WalletTransactionReason.Payout,
            //     payout.Id,
            //     $"Payout rejected: {reason} — balance refunded");
        }
        private async Task RecordTransactionAsync(
             Guid walletId,
             long amount,
             WalletTransactionType type,
             WalletTransactionReason reason,
             Guid? referenceId = null,
             string? note = null)
        {
            await _walletTransactionRepository.CreateAsync(new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = walletId,
                Amount = amount,
                Type = type,
                Reason = reason,
                ReferenceId = referenceId,
                Note = note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        public async Task CreditEarningsAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepository.GetWithStopsAndActivitiesAsync(scheduleId);

            // Group stops by provider (skip null = sightseeing)
            var providerEarnings = schedule.Itinerary.Stops
                .Where(s => s.ProviderId.HasValue)
                .GroupBy(s => s.ProviderId!.Value)
                .Select(g => new
                {
                    ProviderId = g.Key,
                    Total = g.SelectMany(s => s.Activities).Sum(a => a.Price)
                });

            foreach (var earning in providerEarnings)
            {
                var wallet = await _walletRepository.GetByProviderIdAsync(earning.ProviderId);
                if (wallet == null) continue;

                wallet.Balance += earning.Total;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await RecordTransactionAsync(wallet.Id, earning.Total,
                    WalletTransactionType.Credit,
                    WalletTransactionReason.BookingEarning,
                    scheduleId,
                    $"Earnings from schedule {scheduleId}");
            }

            // Agency earnings = BookingItineraries linked to this schedule
            var bookingItineraries = await _bookingItineraryRepository.GetByScheduleIdAsync(scheduleId);
            var agencyGroups = bookingItineraries
                .GroupBy(bi => bi.ItinerarySchedule.Itinerary.AgencyId);

            foreach (var group in agencyGroups)
            {
                var wallet = await _walletRepository.GetByAgencyIdAsync(group.Key);
                if (wallet == null) continue;

                var total = group.Sum(bi => bi.FinalPrice);
                wallet.Balance += total;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await RecordTransactionAsync(wallet.Id, total,
                    WalletTransactionType.Credit,
                    WalletTransactionReason.BookingEarning,
                    scheduleId,
                    $"Agency earnings from schedule {scheduleId}");
            }
        }
    }
}
