namespace TouRest.Application.DTOs.Tracking
{
    public class ItineraryTrackingDTO
    {
        public Guid Id { get; set; }
        public Guid ItineraryScheduleId { get; set; }
        public Guid TrackingId { get; set; }
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
