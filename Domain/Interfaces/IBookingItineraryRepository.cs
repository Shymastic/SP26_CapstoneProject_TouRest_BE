using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IBookingItineraryRepository : IBaseRepository<BookingItinerary>
    {
        Task<List<BookingItinerary>> GetBookingItinerariesByBookingId(Guid bookingId);
        Task<BookingItinerary?> GetBookingItineraryWithDetails(Guid id);
        Task<List<BookingItinerary>> GetByScheduleIdAsync(Guid scheduleId);
    }
}
