using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
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

        private static AgencyScheduleDTO MapToAgencyDTO(ItinerarySchedule s) => new()
        {
            Id            = s.Id,
            ItineraryId   = s.ItineraryId,
            ItineraryName = s.Itinerary?.Name ?? string.Empty,
            StartTime     = s.StartTime,
            EndTime       = s.EndTime,
            Spot          = s.Spot,
            SpotLeft      = s.SpotLeft,
            GuideId       = s.GuideId,
            GuideName     = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
            Status        = s.Status.ToString(),
        };

        public async Task<List<AgencyScheduleDTO>> GetByAgencyIdAsync(Guid agencyId)
        {
            var list = await _repo.GetByAgencyIdAsync(agencyId);
            return list.Select(MapToAgencyDTO).ToList();
        }

        public async Task<List<AgencyScheduleDTO>> GetByGuideIdAsync(Guid guideId)
        {
            var list = await _repo.GetByGuideIdAsync(guideId);
            return list.Select(MapToAgencyDTO).ToList();
        }

        public async Task<List<ProviderScheduleDTO>> GetByProviderIdAsync(Guid providerId)
        {
            var list = await _repo.GetByProviderIdAsync(providerId);
            return list.Select(s => new ProviderScheduleDTO
            {
                Id            = s.Id,
                ItineraryId   = s.ItineraryId,
                ItineraryName = s.Itinerary?.Name ?? string.Empty,
                AgencyName    = s.Itinerary?.Agency?.Name ?? string.Empty,
                StartTime     = s.StartTime,
                EndTime       = s.EndTime,
                Spot          = s.Spot,
                SpotLeft      = s.SpotLeft,
                GuideId       = s.GuideId,
                GuideName     = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
            }).ToList();
        }

        public async Task<bool> DeleteAsync(Guid scheduleId)
        {
            return await _repo.DeleteAsync(scheduleId);
        }

        public async Task AcceptScheduleAsync(Guid scheduleId, Guid guideId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.GuideId != guideId)
                throw new UnauthorizedAccessException("You are not assigned to this schedule");

            if (schedule.Status != ItineraryScheduleStatus.Pending)
                throw new InvalidOperationException("Only pending schedules can be accepted");

            schedule.Status    = ItineraryScheduleStatus.Confirmed;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);
        }

        public async Task RejectScheduleAsync(Guid scheduleId, Guid guideId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.GuideId != guideId)
                throw new UnauthorizedAccessException("You are not assigned to this schedule");

            if (schedule.Status != ItineraryScheduleStatus.Pending)
                throw new InvalidOperationException("Only pending schedules can be rejected");

            // Unassign guide so the agency can reassign another
            schedule.GuideId   = null;
            schedule.Status    = ItineraryScheduleStatus.Pending;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);
        }
    }
}
