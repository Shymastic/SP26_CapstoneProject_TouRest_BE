using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Payout
{
    public class PayoutDTO
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public long Amount { get; set; }
        public string BankAccount { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public PayoutStatus Status { get; set; }
        public string? AdminNote { get; set; }
        public string? PayOSTransferId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class ApprovePayoutRequest
    {
        public string? Note { get; set; }
    }

    public class CompletePayoutRequest
    {
        [Required]
        public string TransferReference { get; set; } = null!; // bank transaction ID
    }

    public class RejectPayoutRequest
    {
        [Required]
        public string Reason { get; set; } = null!;
    }
}
