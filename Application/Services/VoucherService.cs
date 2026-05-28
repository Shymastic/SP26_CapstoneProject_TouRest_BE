using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Voucher;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<IEnumerable<VoucherDTO>> GetAllAsync()
        {
            var vouchers = await _voucherRepository.GetAllAsync();
            return vouchers.Select(MapToDTO);
        }

        public async Task<VoucherDTO?> GetByIdAsync(Guid id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null) return null;

            return MapToDTO(voucher);
        }

        public async Task<VoucherDTO> CreateAsync(VoucherCreateRequest request)
        {
            await ValidateVoucherRequest(request);

            var existingVoucher = await _voucherRepository.GetByCodeAsync(request.Code.Trim());
            if (existingVoucher != null)
                throw new InvalidOperationException("Voucher code already exists.");

            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = request.Code.Trim(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinOrderAmount = request.MinOrderAmount,
                ApplicableType = request.ApplicableType,
                ApplicableId = request.ApplicableId,
                UsageLimit = request.UsageLimit,
                UsedCount = 0,
                ValidFrom = request.ValidFrom,
                ValidTo = request.ValidTo,
                Status = NormalizeStatus(request.Status, request.ValidTo),
                CreatedAt = DateTime.UtcNow
            };

            var createdVoucher = await _voucherRepository.CreateAsync(voucher);
            return MapToDTO(createdVoucher);
        }

        public async Task<VoucherDTO?> UpdateAsync(Guid id, VoucherUpdateRequest request)
        {
            await ValidateVoucherRequest(request);

            var existingVoucher = await _voucherRepository.GetByIdAsync(id);
            if (existingVoucher == null) return null;

            var duplicateVoucher = await _voucherRepository.GetByCodeAsync(request.Code.Trim());
            if (duplicateVoucher != null && duplicateVoucher.Id != id)
                throw new InvalidOperationException("Voucher code already exists.");

            if (request.UsageLimit.HasValue && request.UsageLimit.Value < existingVoucher.UsedCount)
                throw new InvalidOperationException("UsageLimit cannot be less than UsedCount.");

            existingVoucher.Code = request.Code.Trim();
            existingVoucher.Name = request.Name.Trim();
            existingVoucher.Description = request.Description?.Trim();
            existingVoucher.DiscountType = request.DiscountType;
            existingVoucher.DiscountValue = request.DiscountValue;
            existingVoucher.MaxDiscountAmount = request.MaxDiscountAmount;
            existingVoucher.MinOrderAmount = request.MinOrderAmount;
            existingVoucher.ApplicableType = request.ApplicableType;
            existingVoucher.ApplicableId = request.ApplicableId;
            existingVoucher.UsageLimit = request.UsageLimit;
            existingVoucher.ValidFrom = request.ValidFrom;
            existingVoucher.ValidTo = request.ValidTo;
            existingVoucher.Status = NormalizeStatus(request.Status, request.ValidTo);
            existingVoucher.UpdatedAt = DateTime.UtcNow;

            var updatedVoucher = await _voucherRepository.UpdateAsync(existingVoucher);
            return MapToDTO(updatedVoucher);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _voucherRepository.DeleteAsync(id);
        }

        private static async Task ValidateVoucherRequest(VoucherCreateRequest request)
        {
            await Task.CompletedTask;

            if (request.ValidTo <= request.ValidFrom)
                throw new InvalidOperationException("ValidTo must be greater than ValidFrom.");

            if (request.ApplicableType == VoucherApplicableType.All && request.ApplicableId != null)
                throw new InvalidOperationException("ApplicableId must be null when ApplicableType is All.");

            if ((request.ApplicableType == VoucherApplicableType.Service || request.ApplicableType == VoucherApplicableType.Package)
                && request.ApplicableId == null)
                throw new InvalidOperationException("ApplicableId is required when ApplicableType is Service or Package.");

            if (request.UsageLimit.HasValue && request.UsageLimit <= 0)
                throw new InvalidOperationException("UsageLimit must be greater than 0.");

            if (request.DiscountValue < 0)
                throw new InvalidOperationException("DiscountValue must be greater than or equal to 0.");
        }

        private static async Task ValidateVoucherRequest(VoucherUpdateRequest request)
        {
            await Task.CompletedTask;

            if (request.ValidTo <= request.ValidFrom)
                throw new InvalidOperationException("ValidTo must be greater than ValidFrom.");

            if (request.ApplicableType == VoucherApplicableType.All && request.ApplicableId != null)
                throw new InvalidOperationException("ApplicableId must be null when ApplicableType is All.");

            if ((request.ApplicableType == VoucherApplicableType.Service || request.ApplicableType == VoucherApplicableType.Package)
                && request.ApplicableId == null)
                throw new InvalidOperationException("ApplicableId is required when ApplicableType is Service or Package.");

            if (request.UsageLimit.HasValue && request.UsageLimit <= 0)
                throw new InvalidOperationException("UsageLimit must be greater than 0.");

            if (request.DiscountValue < 0)
                throw new InvalidOperationException("DiscountValue must be greater than or equal to 0.");
        }

        private static VoucherStatus NormalizeStatus(VoucherStatus requestedStatus, DateTime validTo)
        {
            if (validTo < DateTime.UtcNow)
                return VoucherStatus.Expired;

            return requestedStatus;
        }

        private static VoucherDTO MapToDTO(Voucher voucher)
        {
            return new VoucherDTO
            {
                Id = voucher.Id,
                Code = voucher.Code,
                Name = voucher.Name,
                Description = voucher.Description,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                MaxDiscountAmount = voucher.MaxDiscountAmount,
                MinOrderAmount = voucher.MinOrderAmount,
                ApplicableType = voucher.ApplicableType,
                ApplicableId = voucher.ApplicableId,
                UsageLimit = voucher.UsageLimit,
                UsedCount = voucher.UsedCount,
                ValidFrom = voucher.ValidFrom,
                ValidTo = voucher.ValidTo,
                Status = voucher.Status,
                CreatedAt = voucher.CreatedAt,
                UpdatedAt = voucher.UpdatedAt
            };
        }

        private static VoucherSummaryDTO MapToSummaryDTO(Voucher voucher)
        {
            return new VoucherSummaryDTO
            {
                Id = voucher.Id,
                Code = voucher.Code,
                Name = voucher.Name,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                ApplicableType = voucher.ApplicableType,
                Status = voucher.Status,
                ValidFrom = voucher.ValidFrom,
                ValidTo = voucher.ValidTo
            };
        }
    }
}
