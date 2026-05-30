using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.DTOs;
using TouRest.Domain.Enums;


namespace TouRest.Application.Interfaces
{
    public interface IAgencyUserService
    {
        Task<bool> IsUserInAgencyAsync(Guid userId, Guid agencyId);
        Task AddUserToAgencyAsync(Guid agencyId, Guid userId, AgencyUserRole role);
        Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);

        Task<List<AgencyUserDTO>> GetTourGuidesAsync(Guid agencyId);
        Task<AgencyUserDTO> CreateGuideAccountAsync(Guid agencyId, CreateGuideRequest request);
        Task<AgencyWithUsersDTO> GetAgencyUsers(Guid agencyId);
        Task<AgencyUserDTO?> GetAgencyUserByUserId(Guid userId);
        Task<List<AgencyGuideDTO>> GetGuidesAsync(Guid agencyId);
    }
}
