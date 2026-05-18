using AutoMapper;
using TouRest.Application.Common.Constants;
using TouRest.Application.Common.Exceptions;
using TouRest.Application.Common.Models;
using TouRest.Application.DTOs.User;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IAgencyUserRepository _agencyUserRepository;

        public UserService(
            IUserRepository userRepository,
            IImageRepository imageRepository,
            IMapper mapper,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            IProviderUserRepository providerUserRepository,
            IAgencyUserRepository agencyUserRepository)
        {
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _providerUserRepository = providerUserRepository;
            _agencyUserRepository = agencyUserRepository;
        }

        public async Task<UserDTO> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with id {id} not found");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<PagedResult<UserDTO>> GetPagedAsync(int page, int pageSize, string? search)
        {
            var (items, total) = await _userRepository.GetPagedAsync(page, pageSize, search);
            return new PagedResult<UserDTO>
            {
                Items      = _mapper.Map<List<UserDTO>>(items),
                TotalCount = total,
                Page       = page,
                PageSize   = pageSize,
            };
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var role = await _roleRepository.GetByCodeAsync(dto.RoleCode.ToUpper());
            if (role == null)
                throw new InvalidOperationException($"Role '{dto.RoleCode}' not found.");

            var user = new User
            {
                Id           = Guid.NewGuid(),
                Username     = dto.Username,
                Email        = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Phone        = dto.Phone,
                RoleId       = role.Id,
                Status       = UserStatus.Active,
                CreatedAt    = DateTime.UtcNow,
            };

            await _userRepository.CreateAsync(user);

            if (dto.RoleCode.Equals(RoleCodes.Provider, StringComparison.OrdinalIgnoreCase) && dto.ProviderId.HasValue)
                await _providerUserRepository.AddUserIntoProvider(dto.ProviderId.Value, user.Id, ProviderUserRole.Manager);

            if (dto.RoleCode.Equals(RoleCodes.Agency, StringComparison.OrdinalIgnoreCase) && dto.AgencyId.HasValue)
                await _agencyUserRepository.AddUserToAgencyAsync(dto.AgencyId.Value, user.Id, AgencyUserRole.Manager);

            var created = await _userRepository.GetByIdAsync(user.Id);
            return _mapper.Map<UserDTO>(created!);
        }

        public async Task<UserDTO> AdminUpdateUserAsync(Guid id, AdminUpdateUserDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException($"User with id {id} not found");

            if (!Enum.TryParse<UserStatus>(dto.Status, ignoreCase: true, out var status))
                throw new InvalidOperationException($"Invalid status '{dto.Status}'.");

            var role = await _roleRepository.GetByCodeAsync(dto.RoleCode.ToUpper());
            if (role == null)
                throw new InvalidOperationException($"Role '{dto.RoleCode}' not found.");

            user.Username      = dto.Username;
            user.FullName      = dto.FullName;
            user.Phone         = dto.Phone;
            user.Status        = status;
            user.RoleId        = role.Id;
            user.DateOfBirth   = dto.DateOfBirth;
            user.AddressDetail = dto.AddressDetail;
            user.CityId        = dto.CityId;
            user.DistrictId    = dto.DistrictId;
            user.UpdatedAt     = DateTime.UtcNow;

            var updated = await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDTO>(updated);
        }

        public async Task<UserDTO> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with id {userId} not found");

            if (dto.Username != null) user.Username = dto.Username;
            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
            if (dto.AddressDetail != null) user.AddressDetail = dto.AddressDetail;
            if (dto.CityId != null) user.CityId = dto.CityId;
            if (dto.DistrictId != null) user.DistrictId = dto.DistrictId;

            if (dto.ImageUrl != null)
            {
                var image = await _imageRepository.CreateAsync(new Image
                {
                    Url  = dto.ImageUrl,
                    Type = ImageType.User
                });
                user.ImageId = image.Id;
            }

            user.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDTO>(updated);
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }
    }
}
