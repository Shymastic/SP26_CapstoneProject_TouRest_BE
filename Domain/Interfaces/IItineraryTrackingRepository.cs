using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryTrackingRepository : IBaseRepository<ItineraryTracking>
    {
        Task<List<ItineraryTracking>> GetByScheduleIdAsync(Guid scheduleId);
        Task<ItineraryTracking?> FindAsync(Guid scheduleId, Guid trackingId, ItineraryTrackingType type);
    }
}
