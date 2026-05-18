using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Payment
{
    public class PaymentDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public long OrderCode { get; set; }
        public long Amount { get; set; }
        public long DiscountAmount { get; set; }
        public long FinalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string? CheckoutUrl { get; set; }
        public string? QrCode { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
