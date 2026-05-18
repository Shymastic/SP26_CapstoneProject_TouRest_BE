using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Helpers;
using TouRest.Application.Common.Models;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly IMapper _mapper;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IStorageService _storageService;

        public AgencyService(
            IAgencyRepository agencyRepository,
            IMapper mapper,
            IImageRepository imageRepository,
            IStorageService storageService)
        {
            _mapper = mapper;
            _agencyRepository = agencyRepository;
            _imageRepository = imageRepository;
            _storageService = storageService;
        }

        public async Task<PagedResult<AgencyDTO>> GetAllAsync(int page, int pageSize)
        {
            var (items, total) = await _agencyRepository.GetPagedAsync(page, pageSize);
            return new PagedResult<AgencyDTO>
            {
                Items      = _mapper.Map<List<AgencyDTO>>(items),
                TotalCount = total,
                Page       = page,
                PageSize   = pageSize,
            };
        }

        public async Task<AgencyDTO> AddAgency(Guid userCreateId, AgencyCreateRequestDTO create)
        {
            ArgumentNullException.ThrowIfNull(create);
            var agency = _mapper.Map<Agency>(create);
            agency.Id = Guid.NewGuid();
            agency.CreateByUserId = userCreateId;

            var startTime = ParseTimeHelper.ParseTime(create.StartTime);
            var endTime = ParseTimeHelper.ParseTime(create.EndTime);

            if (startTime >= endTime)
                throw new ArgumentException("StartTime must be earlier than EndTime");

            agency.StartTime = startTime;
            agency.EndTime = endTime;
            agency.Status = AgencyStatus.Pending;
            agency.CreatedAt = DateTime.UtcNow;
            agency.UpdatedAt = DateTime.UtcNow;

            var createdAgency = await _agencyRepository.CreateAsync(agency);

            if (create.Images != null && create.Images.Count > 0)
            {
                var urls = await _storageService.UploadManyAsync(create.Images);
                for (int i = 0; i < urls.Count; i++)
                {
                    await _imageRepository.CreateAsync(new Image
                    {
                        Id        = Guid.NewGuid(),
                        Url       = urls[i],
                        Type      = ImageType.Agency,
                        TypeId    = createdAgency.Id,
                        PicNumber = i + 1,
                    });
                }
            }

            return _mapper.Map<AgencyDTO>(createdAgency);
        }

        public async Task<bool> DeleteAgency(Guid id)
        {
            return await _agencyRepository.DeleteAsync(id);
        }

        public async Task<AgencyDTO> GetAgencyById(Guid id)
        {
            var agency = await _agencyRepository.GetByIdAsync(id);
            return _mapper.Map<AgencyDTO>(agency);
        }

        public async Task<AgencyDetailDTO?> GetDetailByIdAsync(Guid id)
        {
            var agency = await _agencyRepository.GetByIdAsync(id);
            if (agency == null) return null;

            var images = await _imageRepository.GetByTypeAsync(Domain.Enums.ImageType.Agency, id);

            return new AgencyDetailDTO
            {
                Id             = agency.Id,
                Name           = agency.Name,
                Status         = agency.Status,
                Description    = agency.Description,
                Latitude       = agency.Latitude,
                Longitude      = agency.Longitude,
                Address        = agency.Address,
                StartTime      = agency.StartTime,
                EndTime        = agency.EndTime,
                ContactEmail   = agency.ContactEmail,
                ContactPhone   = agency.ContactPhone,
                CreateByUserId = agency.CreateByUserId,
                CreatedAt      = agency.CreatedAt,
                UpdatedAt      = agency.UpdatedAt,
                Images         = images.Select(i => i.Url).ToList(),
            };
        }
        public async Task<AgencyDTO> GetMyAgency(Guid userId)
        {
            var agency = await _agencyRepository.GetMyAgency(userId);
            return _mapper.Map<AgencyDTO>(agency);
        }
        public async Task<Agency?> GetAgencyByIdWithCreator(Guid agencyId)
        {
            return await _agencyRepository.GetAgencyByIdWithCreator(agencyId);
        }
        public async Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update)
        {
            var existing = await _agencyRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Agency with ID {id} not found.");
            }
            var newStart = existing.StartTime;
            var newEnd = existing.EndTime;

            if (update.StartTime != null)
                newStart = update.StartTime.Value;
            if (update.EndTime != null)
                newEnd = update.EndTime.Value;
            if (newStart >= newEnd)
                throw new Exception("StartTime must be earlier than EndTime");

            existing.StartTime = newStart;
            existing.EndTime = newEnd;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.Description = update.Description ?? existing.Description;
            var updatedAgency = await _agencyRepository.UpdateAsync(existing);
            return _mapper.Map<AgencyDTO>(updatedAgency);
        }


    }

}

