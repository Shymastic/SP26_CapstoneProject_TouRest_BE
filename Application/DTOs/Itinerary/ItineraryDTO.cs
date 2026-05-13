using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Image;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryDTO
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public string? AgencyName { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Price { get; set; }

        public int DurationDays { get; set; }
        public int MaxCapacity { get; set; }
        public int SpotLeft { get; set; }
        public ItineraryStatus Status { get; set; }
        public Guid? TourGuideId { get; set; }
        public string? TourGuideName { get; set; }

        public int StopCount { get; set; }

        public List<ImageDTO> Images { get; set; } = [];

        public List<ItineraryScheduleDTO> Schedules { get; set; } = [];
    }
}
