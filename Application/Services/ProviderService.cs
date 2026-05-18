using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TouRest.Application.Common.Models;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;


namespace TouRest.Application.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public ProviderService(
            IProviderRepository providerRepository,
            IProviderUserRepository providerUserRepository,
            IImageRepository imageRepository,
            IStorageService storageService,
            IMapper mapper)
        {
            _providerRepository = providerRepository;
            _providerUserRepository = providerUserRepository;
            _imageRepository = imageRepository;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<List<ProviderResponse>> GetAllAsync()
        {
            var providers = await _providerRepository.GetAllAsync();
            return providers.Select(MapToResponse).ToList();
        }

        public async Task<PagedResult<ProviderDTO>> GetAllPagedAsync(int page, int pageSize)
        {
            var (items, total) = await _providerRepository.GetPagedAsync(page, pageSize);
            return new PagedResult<ProviderDTO>
            {
                Items      = _mapper.Map<List<ProviderDTO>>(items),
                TotalCount = total,
                Page       = page,
                PageSize   = pageSize,
            };
        }

        public async Task<ProviderResponse?> GetByIdAsync(Guid id)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            return provider == null ? null : MapToResponse(provider);
        }

        public async Task<List<ProviderMapDTO>> GetMapMarkersAsync()
        {
            var providers = await _providerRepository.GetAllAsync();
            return providers
                .Where(p => p.Latitude != 0 && p.Longitude != 0)
                .Select(p => new ProviderMapDTO
                {
                    Id           = p.Id,
                    Name         = p.Name,
                    Latitude     = p.Latitude,
                    Longitude    = p.Longitude,
                    Address      = p.Address,
                    ContactPhone = p.ContactPhone,
                })
                .ToList();
        }

        public async Task<ProviderDetailDTO?> GetDetailByIdAsync(Guid id)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            if (provider == null) return null;

            var images = await _imageRepository.GetByTypeAsync(Domain.Enums.ImageType.Provider, id);

            return new ProviderDetailDTO
            {
                Id            = provider.Id,
                Name          = provider.Name,
                Status        = provider.Status,
                Description   = provider.Description,
                Latitude      = provider.Latitude,
                Longitude     = provider.Longitude,
                Address       = provider.Address,
                StartTime     = provider.StartTime,
                EndTime       = provider.EndTime,
                ContactEmail  = provider.ContactEmail,
                ContactPhone  = provider.ContactPhone,
                CreateByUserId = provider.CreateByUserId,
                CreatedAt     = provider.CreatedAt,
                UpdatedAt     = provider.UpdatedAt,
                Images        = images.Select(i => i.Url).ToList(),
            };
        }

        public async Task<ProviderResponse?> GetByUserIdAsync(Guid userId)
        {
            var providerUser = await _providerUserRepository.GetByUserIdAsync(userId);
            if (providerUser == null) return null;
            var provider = await _providerRepository.GetByIdAsync(providerUser.ProviderId);
            return provider == null ? null : MapToResponse(provider);
        }

        public async Task<ProviderResponse> CreateAsync(Guid currentUserId, CreateProviderRequest request)
        {
            var emailExists = await _providerRepository.ExistsByContactEmailAsync(request.ContactEmail);
            if (emailExists)
            {
                throw new InvalidOperationException("Contact email already exists.");
            }

            var existingProvider = await _providerRepository.GetByCreateByUserIdAsync(currentUserId);
            if (existingProvider != null)
            {
                throw new InvalidOperationException("User has already registered a provider.");
            }

            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = ProviderStatus.Pending,
                CreateByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _providerRepository.AddAsync(provider);

            var providerUser = new ProviderUser
            {
                Id = Guid.NewGuid(),
                ProviderId = provider.Id,
                UserId = currentUserId,
                Role = ProviderUserRole.Manager,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _providerUserRepository.CreateAsync(providerUser);
            await _providerRepository.SaveChangesAsync();

            if (request.Images != null && request.Images.Count > 0)
            {
                var urls = await _storageService.UploadManyAsync(request.Images);
                for (int i = 0; i < urls.Count; i++)
                {
                    await _imageRepository.CreateAsync(new Image
                    {
                        Id        = Guid.NewGuid(),
                        Url       = urls[i],
                        Type      = ImageType.Provider,
                        TypeId    = provider.Id,
                        PicNumber = i + 1,
                    });
                }
            }

            return MapToResponse(provider);
        }

        public async Task<ProviderResponse?> UpdateAsync(Guid id, UpdateProviderRequest request)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            if (provider == null)
            {
                return null;
            }

            var duplicatedEmail = await _providerRepository.GetByContactEmailAsync(request.ContactEmail);
            if (duplicatedEmail != null && duplicatedEmail.Id != id)
            {
                throw new InvalidOperationException("Contact email already exists.");
            }

            provider.Name = request.Name;
            provider.ContactEmail = request.ContactEmail;
            provider.ContactPhone = request.ContactPhone;
            provider.Status = request.Status;
            provider.UpdatedAt = DateTime.UtcNow;

            _providerRepository.Update(provider);
            await _providerRepository.SaveChangesAsync();

            return MapToResponse(provider);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            if (provider == null)
            {
                return false;
            }

            _providerRepository.Remove(provider);
            await _providerRepository.SaveChangesAsync();

            return true;
        }

        private static ProviderResponse MapToResponse(Provider provider)
        {
            return new ProviderResponse
            {
                Id = provider.Id,
                Name = provider.Name,
                Status = provider.Status,
                ContactEmail = provider.ContactEmail,
                ContactPhone = provider.ContactPhone,
                CreatedAt = provider.CreatedAt,
                UpdatedAt = provider.UpdatedAt
            };
        }
    }
}
