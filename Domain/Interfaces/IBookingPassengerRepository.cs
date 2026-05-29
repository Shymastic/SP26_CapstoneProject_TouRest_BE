using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IBookingPassengerRepository : IBaseRepository<BookingPassenger>
    {
        Task<List<BookingPassenger>> GetByBookingIdAsync(Guid bookingId);
    }
}
