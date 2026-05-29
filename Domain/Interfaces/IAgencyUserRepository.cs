using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IAgencyUserRepository
    {
        Task<bool> IsUserInAgencyAsync(Guid agencyId, Guid userId);
        Task AddUserToAgencyAsync(Guid agencyId, Guid userId, AgencyUserRole role);
        Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);
        Task<List<AgencyUser>> GetAgencyUsers(Guid agencyId);
        Task<List<AgencyUser>> GetTourGuidesByAgencyIdAsync(Guid agencyId);
        Task<AgencyUser?> GetAgencyUserByUserId(Guid userId);
        Task<List<AgencyUser>> SearchUsersByAgency(Guid id, SearchUserByAgency search);
    }
    public class SearchUserByAgency {
        public string? Email { get; set; }
        public string? FullName { get; set; }


    }

}
