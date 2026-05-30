using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Refund;

namespace TouRest.Application.Interfaces
{
    public interface IRefundService
    {
        Task<RefundDTO> RequestRefundAsync(RefundRequestDTO request, Guid userId);
        Task<RefundDTO> ReviewRefundAsync(Guid refundId, RefundReviewDTO review);
        Task<RefundDTO> CompleteRefundAsync(Guid refundId);
        Task<RefundDTO> GetRefundByBookingAsync(Guid bookingId);
        Task<CancelBookingResultDTO> CancelAndRefundAsync(Guid bookingId, Guid userId, string? reason);
    }
}
