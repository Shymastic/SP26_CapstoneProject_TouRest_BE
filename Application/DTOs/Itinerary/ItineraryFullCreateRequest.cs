using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryFullCreateRequest
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Duration { get; set; }

        [Range(0, int.MaxValue)]
        public int Price { get; set; }

        // Serialized as JSON string for multipart/form-data compatibility
        public string? StopsJson { get; set; }

        // Populated by the controller after deserializing StopsJson
        public List<ItineraryStopInlineRequest> Stops { get; set; } = [];

        public List<IFormFile>? Images { get; set; }
    }

    public class ItineraryStopInlineRequest
    {
        public int StopOrder { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = null!;

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public Guid? ProviderId { get; set; }

        public Guid? VehicleId { get; set; }

        public List<ItineraryActivityInlineRequest> Activities { get; set; } = [];
    }

    public class ItineraryActivityInlineRequest
    {
        public Guid? ServiceId { get; set; }

        [MaxLength(255)]
        public string? CustomName { get; set; }

        public int ActivityOrder { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        [Range(0, int.MaxValue)]
        public int Price { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
