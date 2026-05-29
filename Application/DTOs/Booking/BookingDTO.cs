using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Booking
{
    public class BookingDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TotalAmount must be greater than 0")]
        public int TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [MaxLength(500)]
        public string? CustomerNote { get; set; }

        [MaxLength(500)]
        public string? InternalNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<BookingPassengerDTO> Passengers { get; set; } = [];
    }
}
