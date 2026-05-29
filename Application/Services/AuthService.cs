using AutoMapper;
using TouRest.Application.Common.Constants;
using TouRest.Application.Common.Exceptions;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Domain.Enums;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Provider;

namespace TouRest.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IAgencyUserRepository _agencyUserRepository;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            IRoleRepository roleRepository,
            IProviderRepository providerRepository,
            IProviderUserRepository providerUserRepository,
            IAgencyRepository agencyRepository,
            IAgencyUserRepository agencyUserRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _providerRepository = providerRepository;
            _providerUserRepository = providerUserRepository;
            _agencyRepository = agencyRepository;
            _agencyUserRepository = agencyUserRepository;
        }

        public async Task<(AuthResponseDTO auth, string refreshToken)> LoginAsync(LoginRequestDTO request)
        {
            // 1. Validate user credentials
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // 2. Check if user exists and password is correct
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            // 3. Check if user account is active
            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("User account is not active");

            // 4. Generate JWT tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays),
            });

            return (new AuthResponseDTO
            {
                AccessToken = accessToken,
                ExpiresIn = AuthConstants.AccessTokenExpiryMinutes * 60
            }, refreshToken);
        }

        public async Task RegisterAsync(RegisterRequestDTO request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            var userRole = await _roleRepository.GetByCodeAsync(RoleCodes.Customer);
            if (userRole == null)
                throw new InvalidOperationException("Default user role not found in database");

            var user = _mapper.Map<User>(request);

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.RoleId = userRole.Id;
            await _userRepository.CreateAsync(user);
        }

        public async Task<(AuthResponseDTO auth, string refreshToken)> RefreshTokenAsync(string refreshTokenValue, Guid userId)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);
            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (refreshToken.UserId != userId)
                throw new UnauthorizedAccessException("Refresh token does not match user");

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            await _refreshTokenRepository.RevokeAsync(refreshTokenValue);

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays)
            });

            return (new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                ExpiresIn = AuthConstants.AccessTokenExpiryMinutes * 60
            }, newRefreshToken);
        }
        public async Task ChangePasswordAsync(Guid userId,ChangePasswordRequestDTO request, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");
            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");
            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            //Log out user from all devices by revoking all refresh tokens
            await LogoutAsync(refreshToken, userId);
            
        }
        public async Task ChangeRoleAsync(Guid userId, ChangeRoleRequestDTO request, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");
            var newRole = await _roleRepository.GetByCodeAsync(request.NewRoleCode);
            if (newRole == null)
                throw new InvalidOperationException("Specified role does not exist");
            user.RoleId = newRole.Id;
            await _userRepository.UpdateAsync(user);
            //Log out user from all devices by revoking all refresh tokens
            await LogoutAsync(refreshToken, userId);
        }
        public async Task LogoutAsync(string refreshTokenValue, Guid userId)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);
            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (refreshToken.UserId != userId)
                throw new UnauthorizedAccessException("Refresh token does not match user");

            await _refreshTokenRepository.RevokeAsync(refreshTokenValue);
        }

        public async Task<MeDTO> GetMeAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            string? subRole = null;
            if (user.Role.Code.Equals("AGENCY", StringComparison.OrdinalIgnoreCase))
            {
                var agencyUser = await _agencyUserRepository.GetAgencyUserByUserId(userId);
                if (agencyUser != null)
                    subRole = agencyUser.Role.ToString().ToLower();
            }

            return new MeDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.Code.ToLower(),
                SubRole = subRole
            };
        }

        public async Task RegisterProviderAccountAsync(Guid createdByUserId, RegisterProviderAccountRequest request)
        {
            var existingProvider = await _providerRepository.GetByContactEmailAsync(request.ContactEmail);
            if (existingProvider != null)
                throw new InvalidOperationException("Provider with this contact email already exists");

            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                Name = request.ProviderName,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = ProviderStatus.Pending,
                CreateByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _providerRepository.AddAsync(provider);
            await _providerRepository.SaveChangesAsync();
        }

        public async Task RegisterAgencyAccountAsync(Guid createdByUserId, RegisterAgencyAccountRequest request)
        {
            var existingAgency = await _agencyRepository.GetByContactEmailAsync(request.ContactEmail);
            if (existingAgency != null)
                throw new InvalidOperationException("Agency with this contact email already exists");

            var agency = new Agency
            {
                Id = Guid.NewGuid(),
                Name = request.AgencyName,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = AgencyStatus.Pending,
                CreateByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _agencyRepository.CreateAsync(agency);
        }
    }
}