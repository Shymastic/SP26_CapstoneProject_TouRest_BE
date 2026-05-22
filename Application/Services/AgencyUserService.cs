using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyUserService : IAgencyUserService
    {
        private readonly IAgencyUserRepository _agencyUserRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AgencyUserService(IAgencyUserRepository agencyUserRepository, IAgencyRepository agencyRepository, IUserRepository userRepository, IMapper mapper)
        {
             _agencyUserRepository = agencyUserRepository;
            _agencyRepository = agencyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        private async Task ValidateAgencyAndUser(Guid agencyId, Guid userId)
        {
            if (agencyId == Guid.Empty)
                throw new ArgumentException("AgencyId cannot be empty", nameof(agencyId));
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            if (await _agencyRepository.GetByIdAsync(agencyId) == null)
                throw new KeyNotFoundException("Agency not found");
            if (await _userRepository.GetByIdAsync(userId) == null)
                throw new KeyNotFoundException("User not found");
        }
        public async Task AddUserToAgencyAsync(Guid agencyId, Guid userId, AgencyUserRole role)
        {
            await ValidateAgencyAndUser(agencyId, userId);
            await _agencyUserRepository.AddUserToAgencyAsync(agencyId, userId, role);
        }

        public async Task<bool> IsUserInAgencyAsync(Guid agencyId, Guid userId)
        {
            await ValidateAgencyAndUser(agencyId, userId);
            return  await _agencyUserRepository.IsUserInAgencyAsync(agencyId, userId);
        }

        public async Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId)
        {
            await ValidateAgencyAndUser(agencyId, userId);
            await _agencyUserRepository.RemoveUserFromAgencyAsync(agencyId, userId);

        }
        public async Task<List<AgencyUserDTO>> GetAgencyUsers(Guid agencyId)
        {
            if (agencyId == Guid.Empty)
                throw new ArgumentException("AgencyId cannot be empty", nameof(agencyId));
            return _mapper.Map<List<AgencyUserDTO>>(await _agencyUserRepository.GetAgencyUsers(agencyId));
        }

        public async Task<AgencyUserDTO?> GetAgencyUserByUserId(Guid userId)
        {
            return _mapper.Map<AgencyUserDTO>(await _agencyUserRepository.GetAgencyUserByUserId(userId));
        }
    }
}
