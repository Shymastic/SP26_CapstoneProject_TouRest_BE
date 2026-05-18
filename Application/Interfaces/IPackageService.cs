using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Package;

namespace TouRest.Application.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageSummaryDTO>> GetAllAsync();
        Task<PackageDTO?> GetByIdAsync(Guid id);
        Task<PackageWithServicesDTO?> GetDetailByIdAsync(Guid id);
        Task<List<PackageWithServicesDTO>> GetByProviderIdAsync(Guid providerId);
        Task<PackageDTO> CreateAsync(PackageCreateRequest request);
        Task<PackageDTO?> UpdateAsync(Guid id, PackageUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
