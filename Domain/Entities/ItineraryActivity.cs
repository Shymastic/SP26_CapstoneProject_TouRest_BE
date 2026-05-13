using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("itinerary_activities")]
    public class ItineraryActivity : BaseEntity
    {
        [Required]
        public Guid ItineraryStopId { get; set; }

        public Guid? ServiceId { get; set; }

        [MaxLength(255)]
        public string? CustomName { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "ActivityOrder must be greater than or equal to 0")]
        public int ActivityOrder { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(0, long.MaxValue)]
        public long Price { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        // Navigation properties
        public ItineraryStop ItineraryStop { get; set; } = null!;
        public Service? Service { get; set; }
    }
}
