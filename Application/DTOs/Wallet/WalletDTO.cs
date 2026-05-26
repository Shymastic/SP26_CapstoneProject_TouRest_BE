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

}
