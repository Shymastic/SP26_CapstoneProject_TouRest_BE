using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Models;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyService
    {

        Task<PagedResult<AgencyDTO>> GetAllAsync(int page, int pageSize);
        Task<AgencyDTO> GetAgencyById(Guid id);
        Task<AgencyDetailDTO?> GetDetailByIdAsync(Guid id);
        Task<AgencyDTO> GetMyAgency(Guid userId);
        Task<Agency?> GetAgencyByIdWithCreator(Guid agencyId);
        Task<AgencyDTO> AddAgency(Guid userCreateId, AgencyCreateRequestDTO create);
        Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update);
        Task<bool> DeleteAgency(Guid id);
    }
}
