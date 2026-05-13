using TouRest.Application.DTOs.ItineraryActivity;

namespace TouRest.Application.DTOs.ItineraryStop
{
    public class StopActivityDTO
    {
        public Guid Id { get; set; }
        public Guid? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? CustomName { get; set; }
        public string? ServiceDescription { get; set; }
        public int ActivityOrder { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Price { get; set; }
        public string? Note { get; set; }
    }

    public class ItineraryStopWithActivitiesDTO
    {
        public Guid Id { get; set; }
        public int StopOrder { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid? ProviderId { get; set; }
        public Guid VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public List<StopActivityDTO> Activities { get; set; } = [];
    }
}
