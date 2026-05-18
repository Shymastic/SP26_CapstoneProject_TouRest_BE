using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.ItineraryStop
{
    public class ItineraryStopCreateRequest
    {
        public int? StopOrder { get; set; }
        public string Name { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string? Address { get; set; }
        public Guid? ProviderId { get; set; }
        public Guid? VehicleId { get; set; }
    }
}
