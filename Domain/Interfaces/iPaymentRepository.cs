using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<Payment?> GetByOrderCodeAsync(long orderCode);
        Task<Payment?> GetActivePaymentByBookingIdAsync(Guid bookingId);
        Task<Payment?> GetLatestPaymentByBookingIdAsync(Guid bookingId);
        Task<List<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId);
    }
}
