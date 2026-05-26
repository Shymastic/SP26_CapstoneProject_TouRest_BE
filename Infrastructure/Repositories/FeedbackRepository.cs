using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Feedback>> GetFeedbacksByBookingItineraryIdAsync(Guid bookingItineraryId)
        {
            return await _context.Feedbacks
                .Where(f => f.BookingItineraryId == bookingItineraryId && f.Status == FeedbackStatus.Active)
                .Include(x => x.BookingItinerary)
                .ThenInclude(x=>x.Booking)
                .ThenInclude(x=>x.User)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Guid?> GetItineraryAgencyIdByFeedbackId(Guid feedbackId)
        {
            return await _context.Feedbacks
                .Where(f => f.Id == feedbackId)
                .Select(f => f.BookingItinerary
                    .ItinerarySchedule
                    .Itinerary.AgencyId)
                .FirstOrDefaultAsync();
        }
        public async Task<RatingSummaryDTO> GetRatingSummaryAsync(Guid itineraryId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.BookingItinerary)
                    .ThenInclude(bi => bi.ItinerarySchedule)
                .Where(f => f.BookingItinerary.ItinerarySchedule.ItineraryId == itineraryId
                    && f.Status == FeedbackStatus.Active)
                .AsNoTracking()
                .ToListAsync();

            if (!feedbacks.Any())
                return new RatingSummaryDTO();

            return new RatingSummaryDTO
            {
                AverageRating = Math.Round(feedbacks.Average(f => f.Rating), 1),
                TotalReviews = feedbacks.Count,
                RatingCounts = feedbacks
                    .GroupBy(f => f.Rating)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
        public async Task<List<Feedback>> GetFeedbacksByItineraryIdAsync(Guid itineraryId)
        {
            return await _context.Feedbacks
                .Include(x => x.BookingItinerary)
                    .ThenInclude(bi => bi.Booking)
                        .ThenInclude(b => b.User)
                .Where(f => f.BookingItinerary.ItinerarySchedule.ItineraryId == itineraryId && f.Status == FeedbackStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Feedback?> GetFeedback(Guid id)
        {
            return await _context.Feedbacks
                .Include(x => x.BookingItinerary)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        public async Task<List<Feedback>> GetFeedbacks(FeedbackSearch search)
        {
            var query = _context.Feedbacks
                .Include(x => x.BookingItinerary).ThenInclude(bi => bi.Booking)
                .AsNoTracking()
                .AsQueryable();
            if(!string.IsNullOrEmpty(search.BookingCode))
            {
                query = query.Where(f => f.BookingItinerary.Booking.Code.Contains(search.BookingCode));
            }
            if(search.ItemType.HasValue)
            {
                query = query.Where(f => f.ItemType == search.ItemType.Value);
            }
            if(search.Rating.HasValue)
            {
                query = query.Where(f => f.Rating == search.Rating.Value);
            }
            if(!string.IsNullOrEmpty(search.Title))
            {
                query = query.Where(f => f.Title.Contains(search.Title));
            }
            if(search.IsAnonymous.HasValue)
            {
                query = query.Where(f => f.IsAnonymous == search.IsAnonymous.Value);
            }
            if(search.Status.HasValue)
            {
                query = query.Where(f => f.Status == search.Status.Value);
            }
            return await query.ToListAsync();
        }
        public async Task<(double AverageRating, int TotalReviews)> GetRatingStatsByAgencyIdAsync(Guid agencyId)
        {
            var ratings = await _context.Feedbacks
                .Where(f => f.BookingItinerary.ItinerarySchedule.Itinerary.AgencyId == agencyId
                    && f.Status == FeedbackStatus.Active)
                .Select(f => f.Rating)
                .ToListAsync();

            if (!ratings.Any()) return (0, 0);
            return (Math.Round(ratings.Average(), 1), ratings.Count);
        }
    }
}
