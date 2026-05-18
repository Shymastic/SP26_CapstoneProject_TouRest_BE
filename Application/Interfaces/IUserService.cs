using TouRest.Application.Common.Models;
using TouRest.Application.DTOs.User;

namespace TouRest.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetByIdAsync(Guid id);
        Task<PagedResult<UserDTO>> GetPagedAsync(int page, int pageSize, string? search);
        Task<UserDTO> CreateUserAsync(CreateUserDTO dto);
        Task<UserDTO> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto);
        Task<UserDTO> AdminUpdateUserAsync(Guid id, AdminUpdateUserDTO dto);
        Task<List<UserDTO>> GetAllAsync();
    }
}
