using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<List<Booking>> GetBookingsByUserIdAsync(Guid userId, BookingStatus? status = null);
        Task<Booking?> GetBookingWithItineraries(Guid bookingId);
    }
}
