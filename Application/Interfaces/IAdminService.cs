using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Feedback;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.DTOs.Service;
using TouRest.Application.DTOs.User;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IAdminService
    {
        Task BanUserAsync(Guid userId);
        Task UnbanUserAsync(Guid userId);
        Task PromoteToAdminAsync(Guid userId);
        Task DemoteFromAdminAsync(Guid userId, Guid roleId, UserStatus? status);
        Task ApproveAgency(Guid agencyId);
        Task RejectAgency(Guid agencyId);
        Task ApproveProvider(Guid providerId);
        Task RejectProvider(Guid providerId);
        Task<List<AgencyDTO>> GetAgencies(AgencySearch search);
        Task<List<ProviderDTO>> GetProviders(ProviderSearch search);
        Task<List<UserDTO>> GetUsers(UserSearch search);
        Task CreateAgencyAccount(Guid agencyId, CreateAgencyAccountRequest request);
        Task CreateProviderAccount(Guid providerId, CreateProviderAccountRequest request);
        Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search);
        Task HideFeedback(Guid feedbackId);
        Task DeleteFeedback(Guid feedbackId);
        Task Dashboard();
    }
}
