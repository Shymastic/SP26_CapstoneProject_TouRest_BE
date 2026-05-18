using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("bookings")]
    public class Booking : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "TotalAmount must be greater than 0")]
        public long TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [MaxLength(500)]
        public string? CustomerNote { get; set; }

        [MaxLength(500)]
        public string? InternalNote { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<BookingItinerary> BookingItineraries { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
    }
}
