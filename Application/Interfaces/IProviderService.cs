using TouRest.Application.Common.Models;
using TouRest.Application.DTOs.Provider;

namespace TouRest.Application.Interfaces
{
    public interface IProviderService
    {
        Task<List<ProviderResponse>> GetAllAsync();
        Task<PagedResult<ProviderDTO>> GetAllPagedAsync(int page, int pageSize);
        Task<ProviderResponse?> GetByIdAsync(Guid id);
        Task<ProviderDetailDTO?> GetDetailByIdAsync(Guid id);
        Task<ProviderResponse?> GetByUserIdAsync(Guid userId);
        Task<List<ProviderMapDTO>> GetMapMarkersAsync();
        Task<ProviderResponse> CreateAsync(Guid currentUserId, CreateProviderRequest request);
        Task<ProviderResponse?> UpdateAsync(Guid id, UpdateProviderRequest request);
        Task<bool> DeleteAsync(Guid id);
        //Task<ProviderResponse> CreateAsync(CreateProviderRequest request);
    }
}
