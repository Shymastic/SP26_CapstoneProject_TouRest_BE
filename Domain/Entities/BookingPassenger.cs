using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("booking_passengers")]
    public class BookingPassenger : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(12)]
        public string IdNumber { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }

        // Navigation
        public Booking Booking { get; set; } = null!;
    }
}
