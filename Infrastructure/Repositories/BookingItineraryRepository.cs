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
    public class BookingItineraryRepository : BaseRepository<BookingItinerary>, IBookingItineraryRepository
    {
        public BookingItineraryRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<BookingItinerary>> GetBookingItinerariesByBookingId(Guid bookingId)
        {
            return await _context.BookingItineraries
                .Where(x => x.BookingId == bookingId)
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Itinerary)
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Guide)
                .Include(bi => bi.Voucher)
                .ToListAsync();
        }
        public async Task<BookingItinerary?> GetBookingItineraryWithDetails(Guid id)
        {
            return await _context.BookingItineraries
                .Include(bi => bi.Booking)
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Itinerary)
                .FirstOrDefaultAsync(bi => bi.Id == id);
        }

        public async Task<BookingItinerary?> GetCompletedByUserAndItinerary(Guid userId, Guid itineraryId)
        {
            return await _context.BookingItineraries
                .Include(bi => bi.Booking)
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Itinerary)
                .Where(bi => bi.Booking.UserId == userId
                          && bi.ItinerarySchedule.Itinerary.Id == itineraryId
                          && bi.Status == BookingItineraryStatus.Completed)
                .OrderByDescending(bi => bi.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<List<BookingItinerary>> GetByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.BookingItineraries
                .Include(bi => bi.ItinerarySchedule)
                    .ThenInclude(s => s.Itinerary)
                .Where(bi => bi.ItineraryScheduleId == scheduleId)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}
