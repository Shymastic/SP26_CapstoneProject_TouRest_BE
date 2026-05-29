using TouRest.Domain.DTOs;

namespace TouRest.Application.Interfaces
{
    public interface IProviderStaffService
    {
        Task<List<ProviderTourGroupDTO>> GetTourGroupsAsync(Guid providerId);
        Task<List<ProviderPatientDTO>> GetPatientsAsync(Guid scheduleId);
        Task<List<ProviderPassengerDTO>> GetPassengersAsync(Guid scheduleId);
        Task<ProviderPassengerDTO> SendMedicalResultAsync(Guid passengerId, Guid scheduleId, Guid providerId, string? notes, List<string> imageUrls);
        Task<BookingStopMedicalResultDTO> GetBookingStopResultsAsync(Guid bookingId, Guid stopId);
    }
}
