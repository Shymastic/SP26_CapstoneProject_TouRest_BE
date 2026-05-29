using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Wallet
{
    public class WalletDTO
    {
        public Guid Id { get; set; }
        public long Balance { get; set; }
        public long PendingBalance { get; set; }
        public string OwnerType { get; set; } = null!;
    }

    public class WalletTransactionDTO
    {
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public WalletTransactionType Type { get; set; }
        public WalletTransactionReason Reason { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SavedBankDTO
    {
        public string BankAccount { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public DateTime LastUsedAt { get; set; }
    }

    public class PayoutRequestDTO
    {
        [Required]
        [Range(10000, long.MaxValue, ErrorMessage = "Minimum payout is 10,000đ")]
        public long Amount { get; set; }
        [Required]
        public string BankAccount { get; set; } = null!;
        [Required]
        public string BankName { get; set; } = null!;
        [Required]
        public string AccountHolder { get; set; } = null!;
    }
    public class AddFundsRequestDTO
    {
        [Required]
        [Range(10000, long.MaxValue, ErrorMessage = "Minimum amount is 10,000đ")]
        public long Amount { get; set; }
    }
    public class WalletSummaryDTO
    {
        public long TotalBalance { get; set; }
        public long TotalPendingBalance { get; set; }
        public List<WalletTransactionDTO> RecentTransactions { get; set; } = new();
    }
    public class WalletTransactionDetailDTO
    {
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public WalletTransactionType Type { get; set; }
        public WalletTransactionReason Reason { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ReferenceDetails { get; set; } // e.g. booking code, service name, etc.
    }
    public class WalletTransactionsPageDTO
    {
        public List<WalletTransactionDetailDTO> Transactions { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalTransactions { get; set; }
    }
    public class PayoutRequestDetailDTO
    {
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public string BankAccount { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = null!; // e.g. Pending, Approved, Rejected
        public DateTime? ProcessedAt { get; set; }
        public string? RejectionReason { get; set; }

    }
    public class WithdrawFundsRequestDTO
    {
        [Required]
        [Range(10000, long.MaxValue, ErrorMessage = "Minimum withdrawal is 10,000đ")]
        public long Amount { get; set; }
        [Required]
        public string BankAccount { get; set; } = null!;
        [Required]
        public string BankName { get; set; } = null!;
        [Required]
        public string AccountHolder { get; set; } = null!;
    }
    public class WithdrawFundsResponseDTO
    {
        public Guid TransactionId { get; set; }
        public long Amount { get; set; }
        public string BankAccount { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = null!; // e.g. Pending, Approved, Rejected
    }
    public class AddBankRequestDTO
    {
        public string BankAccount { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public string BankName { get; set; } = null!;

    }
}

