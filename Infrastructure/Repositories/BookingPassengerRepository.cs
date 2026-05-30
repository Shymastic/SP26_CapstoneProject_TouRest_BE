using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class BookingPassengerRepository : BaseRepository<BookingPassenger>, IBookingPassengerRepository
    {
        public BookingPassengerRepository(AppDbContext context) : base(context) { }

        public async Task<List<BookingPassenger>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.BookingPassengers
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();
        }
    }
}
