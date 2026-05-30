using TouRest.Application.DTOs.Tracking;
using TouRest.Domain.Enums;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryTrackingService
    {
        Task<List<ItineraryTrackingDTO>> GetByScheduleIdAsync(Guid scheduleId);
        Task<ItineraryTrackingDTO> TrackAsync(TrackRequest request);
        Task<bool> UntrackAsync(Guid scheduleId, Guid trackingId, ItineraryTrackingType type);
    }
}
