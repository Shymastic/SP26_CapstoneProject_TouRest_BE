using TouRest.Application.DTOs.Vehicle;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<List<VehicleDTO>> GetMyVehiclesAsync(Guid agencyId)
        {
            var vehicles = await _vehicleRepository.GetByAgencyIdAsync(agencyId);
            return vehicles.Select(ToDTO).ToList();
        }

        public async Task<VehicleDTO> CreateAsync(Guid agencyId, VehicleCreateRequest request)
        {
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                AgencyId = agencyId,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Capacity = request.Capacity,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow,
            };
            var saved = await _vehicleRepository.CreateAsync(vehicle);
            return ToDTO(saved);
        }

        public async Task<VehicleDTO> UpdateAsync(Guid id, Guid agencyId, VehicleUpdateRequest request)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Vehicle not found");
            if (vehicle.AgencyId != agencyId)
                throw new UnauthorizedAccessException("Not your vehicle");

            vehicle.Name = request.Name.Trim();
            vehicle.Description = request.Description?.Trim();
            vehicle.Capacity = request.Capacity;
            vehicle.Type = request.Type;
            var updated = await _vehicleRepository.UpdateAsync(vehicle);
            return ToDTO(updated);
        }

        public async Task DeleteAsync(Guid id, Guid agencyId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Vehicle not found");
            if (vehicle.AgencyId != agencyId)
                throw new UnauthorizedAccessException("Not your vehicle");
            await _vehicleRepository.DeleteAsync(id);
        }

        private static VehicleDTO ToDTO(Vehicle v) => new()
        {
            Id = v.Id,
            Name = v.Name,
            Description = v.Description,
            Capacity = v.Capacity,
            Type = v.Type,
            AgencyId = v.AgencyId,
        };
    }
}
