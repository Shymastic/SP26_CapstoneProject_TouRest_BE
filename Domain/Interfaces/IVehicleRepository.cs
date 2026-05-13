using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IVehicleRepository : IBaseRepository<Vehicle>
    {
        Task<List<Vehicle>> GetByAgencyIdAsync(Guid agencyId);
    }
}
