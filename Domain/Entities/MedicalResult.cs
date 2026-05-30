using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("medical_results")]
    public class MedicalResult : BaseEntity
    {
        [Required]
        public Guid PassengerId { get; set; }

        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public Guid ProviderId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        // Navigation
        public BookingPassenger Passenger { get; set; } = null!;
        public ItinerarySchedule Schedule { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
        public ICollection<MedicalResultImage> Images { get; set; } = [];
    }
}
