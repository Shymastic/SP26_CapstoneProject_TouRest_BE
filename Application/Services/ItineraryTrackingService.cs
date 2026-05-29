using TouRest.Application.DTOs.Tracking;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryTrackingService : IItineraryTrackingService
    {
        private readonly IItineraryTrackingRepository _repo;

        public ItineraryTrackingService(IItineraryTrackingRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ItineraryTrackingDTO>> GetByScheduleIdAsync(Guid scheduleId)
        {
            var records = await _repo.GetByScheduleIdAsync(scheduleId);
            return records.Select(MapToDTO).ToList();
        }

        public async Task<ItineraryTrackingDTO> TrackAsync(TrackRequest request)
        {
            var existing = await _repo.FindAsync(request.ItineraryScheduleId, request.TrackingId, request.Type);
            if (existing != null)
                return MapToDTO(existing);

            var entity = new ItineraryTracking
            {
                Id                  = Guid.NewGuid(),
                ItineraryScheduleId = request.ItineraryScheduleId,
                TrackingId          = request.TrackingId,
                Type                = request.Type
            };

            var created = await _repo.CreateAsync(entity);
            return MapToDTO(created);
        }

        public async Task<bool> UntrackAsync(Guid scheduleId, Guid trackingId, ItineraryTrackingType type)
        {
            var existing = await _repo.FindAsync(scheduleId, trackingId, type);
            if (existing == null) return false;
            return await _repo.DeleteAsync(existing.Id);
        }

        private static ItineraryTrackingDTO MapToDTO(ItineraryTracking e) => new()
        {
            Id = e.Id,
            ItineraryScheduleId = e.ItineraryScheduleId,
            TrackingId = e.TrackingId,
            Type = (int)e.Type,
            CreatedAt = e.CreatedAt
        };
    }
}
