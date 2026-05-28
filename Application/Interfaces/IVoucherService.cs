using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Voucher;

namespace TouRest.Application.Interfaces
{
    public interface IVoucherService
    {
        Task<IEnumerable<VoucherDTO>> GetAllAsync();
        Task<VoucherDTO?> GetByIdAsync(Guid id);
        Task<VoucherDTO> CreateAsync(VoucherCreateRequest request);
        Task<VoucherDTO?> UpdateAsync(Guid id, VoucherUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
