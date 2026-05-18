using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Payment
{
    public class CreateQrPaymentRequest
    {
        [Required]
        public Guid BookingId { get; set; }

        public string? IpAddr { get; set; }

        public int ExpireMinutes { get; set; } = 15;
    }
}
