using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Booking>> GetBookingsByUserIdAsync(Guid userId, BookingStatus? status = null)
        {
            var query = _context.Bookings.Where(b => b.UserId == userId);
            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);
            return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }
        public async Task<Booking?> GetBookingWithItineraries(Guid bookingId)
        {
            return await _context.Bookings
                .Include(b => b.BookingItineraries)
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }
    }
}
