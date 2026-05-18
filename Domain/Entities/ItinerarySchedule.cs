using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("itinerary_schedule")]
    public class ItinerarySchedule : BaseEntity
    {
        [Required]
        public Guid ItineraryId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int Spot { get; set; }

        [Required]
        public int SpotLeft { get; set; }

        public Guid? GuideId { get; set; }

        // Navigation properties
        public Itinerary Itinerary { get; set; } = null!;
        public User? Guide { get; set; }
    }
}
