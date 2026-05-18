using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Package;
using TouRest.Application.DTOs.PackageService;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageServiceRepository _packageServiceRepository;

        public PackageService(
            IPackageRepository packageRepository,
            IPackageServiceRepository packageServiceRepository)
        {
            _packageRepository = packageRepository;
            _packageServiceRepository = packageServiceRepository;
        }

        public async Task<IEnumerable<PackageSummaryDTO>> GetAllAsync()
        {
            var packages = await _packageRepository.GetAllAsync();
            return packages.Select(MapToSummaryDTO);
        }

        public async Task<PackageDTO?> GetByIdAsync(Guid id)
        {
            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null) return null;

            var dto = MapToDTO(package);
            var packageServices = await _packageServiceRepository.GetPackageServicesByPackageId(id);
            dto.ServiceIds = packageServices.Select(ps => ps.ServiceId).ToList();
            return dto;
        }

        public async Task<PackageWithServicesDTO?> GetDetailByIdAsync(Guid id)
        {
            var p = await _packageRepository.GetByIdWithServicesAsync(id);
            if (p == null) return null;

            return new PackageWithServicesDTO
            {
                Id        = p.Id,
                Code      = p.Code,
                Name      = p.Name,
                BasePrice = p.BasePrice,
                Status    = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Services  = p.PackageServices.Select(ps => new PackageServiceDTO
                {
                    PackageId              = ps.PackageId,
                    ServiceId              = ps.ServiceId,
                    SortOrder              = ps.SortOrder,
                    ServiceName            = ps.Service.Name,
                    ServiceDescription     = ps.Service.Description,
                    ServicePrice           = ps.Service.Price,
                    ServiceDurationMinutes = ps.Service.DurationMinutes,
                    ServiceStatus          = ps.Service.Status,
                    ServiceBasePrice       = ps.Service.BasePrice,
                }).ToList()
            };
        }

        public async Task<List<PackageWithServicesDTO>> GetByProviderIdAsync(Guid providerId)
        {
            var packages = await _packageRepository.GetByProviderIdWithServicesAsync(providerId);
            return packages.Select(p => new PackageWithServicesDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                BasePrice = p.BasePrice,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Services = p.PackageServices.Select(ps => new PackageServiceDTO
                {
                    PackageId = ps.PackageId,
                    ServiceId = ps.ServiceId,
                    SortOrder = ps.SortOrder,
                    ServiceName = ps.Service.Name,
                    ServiceDescription = ps.Service.Description,
                    ServicePrice = ps.Service.Price,
                    ServiceDurationMinutes = ps.Service.DurationMinutes,
                    ServiceStatus = ps.Service.Status,
                    ServiceBasePrice = ps.Service.BasePrice,
                }).ToList()
            }).ToList();
        }

        public async Task<PackageDTO> CreateAsync(PackageCreateRequest request)
        {
            var existing = await _packageRepository.GetByCodeAsync(request.Code.Trim());
            if (existing != null)
                throw new InvalidOperationException("Package code already exists.");

            var package = new Package
            {
                Id = Guid.NewGuid(),
                Code = request.Code.Trim(),
                Name = request.Name.Trim(),
                BasePrice = request.BasePrice,
                Status = PackageStatus.Archived,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _packageRepository.CreateAsync(package);

            var serviceIds = request.ServiceIds ?? [];
            for (int i = 0; i < serviceIds.Count; i++)
            {
                await _packageServiceRepository.CreateAsync(new Domain.Entities.PackageService
                {
                    PackageId = result.Id,
                    ServiceId = serviceIds[i],
                    SortOrder = i,
                });
            }

            var dto = MapToDTO(result);
            dto.ServiceIds = serviceIds;
            return dto;
        }

        public async Task<PackageDTO?> UpdateAsync(Guid id, PackageUpdateRequest request)
        {
            var existing = await _packageRepository.GetByIdAsync(id);
            if (existing == null) return null;

            var duplicate = await _packageRepository.GetByCodeAsync(request.Code.Trim());
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("Package code already exists.");

            existing.Code = request.Code.Trim();
            existing.Name = request.Name.Trim();
            existing.BasePrice = request.BasePrice;
            existing.Status = request.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _packageRepository.UpdateAsync(existing);

            var dto = MapToDTO(result);

            if (request.ServiceIds != null)
            {
                var current = await _packageServiceRepository.GetPackageServicesByPackageId(id);
                var currentIds = current.Select(ps => ps.ServiceId).ToHashSet();
                var requestIds = request.ServiceIds.ToHashSet();

                foreach (var ps in current.Where(ps => !requestIds.Contains(ps.ServiceId)))
                    await _packageServiceRepository.DeleteAsync(id, ps.ServiceId);

                var toAdd = requestIds.Except(currentIds).ToList();
                for (int i = 0; i < toAdd.Count; i++)
                    await _packageServiceRepository.CreateAsync(new Domain.Entities.PackageService
                    {
                        PackageId = id,
                        ServiceId = toAdd[i],
                        SortOrder = current.Count + i,
                    });

                dto.ServiceIds = request.ServiceIds;
            }

            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _packageRepository.DeleteAsync(id);
        }

        // ================= MAP =================

        private static PackageDTO MapToDTO(Package p)
        {
            return new PackageDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                BasePrice = p.BasePrice,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }

        private static PackageSummaryDTO MapToSummaryDTO(Package p)
        {
            return new PackageSummaryDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                BasePrice = p.BasePrice,
                Status = p.Status
            };
        }
    }
}
