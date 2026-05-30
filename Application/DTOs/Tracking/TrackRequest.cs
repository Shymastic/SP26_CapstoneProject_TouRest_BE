using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Tracking
{
    public class TrackRequest
    {
        public Guid ItineraryScheduleId { get; set; }
        public Guid TrackingId { get; set; }
        public ItineraryTrackingType Type { get; set; }
    }
}
