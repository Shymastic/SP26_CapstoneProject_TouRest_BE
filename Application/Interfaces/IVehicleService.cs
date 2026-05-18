using TouRest.Application.DTOs.Vehicle;

namespace TouRest.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<List<VehicleDTO>> GetMyVehiclesAsync(Guid agencyId);
        Task<VehicleDTO> CreateAsync(Guid agencyId, VehicleCreateRequest request);
        Task<VehicleDTO> UpdateAsync(Guid id, Guid agencyId, VehicleUpdateRequest request);
        Task DeleteAsync(Guid id, Guid agencyId);
    }
}
