using TouRest.Application.DTOs.Itinerary;
using TouRest.Domain.Enums;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryScheduleService
    {
        Task<List<ItineraryScheduleDTO>> GetByItineraryIdAsync(Guid itineraryId);
        Task<ItineraryScheduleDTO> AddAsync(Guid itineraryId, ItineraryScheduleCreateRequest request);
        Task<bool> DeleteAsync(Guid scheduleId);
        Task<List<AgencyScheduleDTO>> GetByAgencyIdAsync(Guid agencyId);
        Task<List<AgencyScheduleDTO>> GetByGuideIdAsync(Guid guideId);
        Task<List<ProviderScheduleDTO>> GetByProviderIdAsync(Guid providerId);
        Task UpdateStatusAsync(Guid scheduleId, ItineraryScheduleStatus status);
    }
}
