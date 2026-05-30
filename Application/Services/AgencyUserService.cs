using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.DTOs;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyUserService : IAgencyUserService
    {
        private readonly IAgencyUserRepository _agencyUserRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public AgencyUserService(
            IAgencyUserRepository agencyUserRepository,
            IAgencyRepository agencyRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _agencyUserRepository = agencyUserRepository;
            _agencyRepository = agencyRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
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
        public async Task<AgencyWithUsersDTO> GetAgencyUsers(Guid agencyId)
        {
            if (agencyId == Guid.Empty)
                throw new ArgumentException("AgencyId cannot be empty", nameof(agencyId));

            var agency = await _agencyRepository.GetByIdAsync(agencyId);

            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID '{agencyId}' not found.");

            return _mapper.Map<AgencyWithUsersDTO>(agency);
        }

        public async Task<AgencyUserDTO?> GetAgencyUserByUserId(Guid userId)
        {
            return _mapper.Map<AgencyUserDTO>(await _agencyUserRepository.GetAgencyUserByUserId(userId));
        }
        
        public async Task<List<AgencyGuideDTO>> GetGuidesAsync(Guid agencyId)
        {
            if (agencyId == Guid.Empty)
                throw new ArgumentException("AgencyId cannot be empty", nameof(agencyId));
            return await _agencyUserRepository.GetGuidesByAgencyIdAsync(agencyId);
        }

        public async Task<List<AgencyUserDTO>> GetTourGuidesAsync(Guid agencyId)
        {
            if (agencyId == Guid.Empty)
                throw new ArgumentException("AgencyId cannot be empty", nameof(agencyId));
            return _mapper.Map<List<AgencyUserDTO>>(await _agencyUserRepository.GetTourGuidesByAgencyIdAsync(agencyId));
        }

        public async Task<AgencyUserDTO> CreateGuideAccountAsync(Guid agencyId, CreateGuideRequest request)
        {
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
                throw new InvalidOperationException("A user with this email already exists");

            var agencyRole = await _roleRepository.GetByCodeAsync(RoleCodes.Agency);
            if (agencyRole == null)
                throw new InvalidOperationException("AGENCY role not found");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = agencyRole.Id,
                Status = UserStatus.Active,
            };
            await _userRepository.CreateAsync(user);
            await _agencyUserRepository.AddUserToAgencyAsync(agencyId, user.Id, AgencyUserRole.TourGuide);

            return new AgencyUserDTO
            {
                AgencyId = agencyId,
                UserId = user.Id,
                UserFullName = user.FullName ?? user.Username,
                Email = user.Email,
                Role = AgencyUserRole.TourGuide.ToString(),
            };
        }
    }
}
