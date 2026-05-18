using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryScheduleService : IItineraryScheduleService
    {
        private readonly IItineraryScheduleRepository _repo;

        public ItineraryScheduleService(IItineraryScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ItineraryScheduleDTO>> GetByItineraryIdAsync(Guid itineraryId)
        {
            var list = await _repo.GetByItineraryIdAsync(itineraryId);
            return list.Select(MapToDTO).ToList();
        }

        public async Task<ItineraryScheduleDTO> AddAsync(Guid itineraryId, ItineraryScheduleCreateRequest request)
        {
            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.");
            if (request.Spot < 1)
                throw new ArgumentException("Spot must be at least 1.");

            var schedule = new ItinerarySchedule
            {
                Id          = Guid.NewGuid(),
                ItineraryId = itineraryId,
                StartTime   = request.StartTime,
                EndTime     = request.EndTime,
                Spot        = request.Spot,
                SpotLeft    = request.Spot,
                GuideId     = request.GuideId,
                CreatedAt   = DateTime.UtcNow,
            };
            var saved = await _repo.CreateAsync(schedule);

            // Reload with Guide navigation to populate GuideName
            var full = await _repo.GetByIdWithGuideAsync(saved.Id);
            return MapToDTO(full ?? saved);
        }

        private static ItineraryScheduleDTO MapToDTO(ItinerarySchedule s) => new()
        {
            Id          = s.Id,
            ItineraryId = s.ItineraryId,
            StartTime   = s.StartTime,
            EndTime     = s.EndTime,
            Spot        = s.Spot,
            SpotLeft    = s.SpotLeft,
            GuideId     = s.GuideId,
            GuideName   = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
        };

        public async Task<bool> DeleteAsync(Guid scheduleId)
        {
            return await _repo.DeleteAsync(scheduleId);
        }
    }
}
