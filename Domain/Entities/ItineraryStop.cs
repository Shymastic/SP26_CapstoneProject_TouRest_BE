using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("itinerary_stops")]
    public class ItineraryStop : BaseEntity
    {
        [Required]
        public Guid ItineraryId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "StopOrder must be greater than or equal to 0")]
        public int StopOrder { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
        public Guid? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public Guid? ProviderId { get; set; }

        // Navigation properties
        public Itinerary Itinerary { get; set; } = null!;
        public Provider? Provider { get; set; }
        public ICollection<ItineraryActivity> Activities { get; set; } = [];
    }
}
