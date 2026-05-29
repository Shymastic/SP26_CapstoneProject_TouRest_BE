using TouRest.Application.Interfaces;
using TouRest.Domain.DTOs;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ProviderStaffService : IProviderStaffService
    {
        private readonly IProviderStaffRepository _repo;

        public ProviderStaffService(IProviderStaffRepository repo)
        {
            _repo = repo;
        }

        public Task<List<ProviderTourGroupDTO>> GetTourGroupsAsync(Guid providerId)
            => _repo.GetTourGroupsAsync(providerId);

        public Task<List<ProviderPatientDTO>> GetPatientsAsync(Guid scheduleId)
            => _repo.GetPatientsAsync(scheduleId);

        public Task<List<ProviderPassengerDTO>> GetPassengersAsync(Guid scheduleId)
            => _repo.GetPassengersAsync(scheduleId);

        public Task<ProviderPassengerDTO> SendMedicalResultAsync(Guid passengerId, Guid scheduleId, Guid providerId, string? notes, List<string> imageUrls)
            => _repo.SendMedicalResultAsync(passengerId, scheduleId, providerId, notes, imageUrls);

        public Task<BookingStopMedicalResultDTO> GetBookingStopResultsAsync(Guid bookingId, Guid stopId)
            => _repo.GetBookingStopResultsAsync(bookingId, stopId);
    }
}
