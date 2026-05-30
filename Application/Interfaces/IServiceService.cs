using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Service;
using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface IServiceService
    {
        Task<ServiceDTO?> GetServiceById(Guid id);
        Task<IEnumerable<ServiceDTO>> GetAllServices();
        Task<IEnumerable<ServiceDTO>> GetServicesByProviderId(Guid providerId);
        Task<IEnumerable<ServiceDTO>> GetMyProviderServices(Guid userId);
        Task<ServiceDTO> CreateService(ServiceCreateRequest request);
        Task<ServiceDTO?> UpdateService(Guid id, ServiceUpdateRequest request);
            Task<bool> DeleteService(Guid id);

    }
}
