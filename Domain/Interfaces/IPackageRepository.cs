using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IPackageRepository : IBaseRepository<Package>
    {
        Task<Package?> GetByCodeAsync(string code);
        Task<List<Package>> GetByProviderIdWithServicesAsync(Guid providerId);
        Task<Package?> GetByIdWithServicesAsync(Guid id);
    }
}
