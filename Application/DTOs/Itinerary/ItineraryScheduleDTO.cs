namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryScheduleDTO
    {
        public Guid Id { get; set; }
        public Guid ItineraryId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Spot { get; set; }
        public int SpotLeft { get; set; }
        public Guid? GuideId { get; set; }
        public string? GuideName { get; set; }
    }

    public class ItineraryScheduleCreateRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Spot { get; set; }
        public Guid? GuideId { get; set; }
    }
}
