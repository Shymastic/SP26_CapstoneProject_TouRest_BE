using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IFeedbackRepository : IBaseRepository<Feedback>
    {
        Task<List<Feedback>> GetFeedbacksByBookingItineraryIdAsync(Guid bookingItineraryId);
        Task<Feedback?> GetFeedback(Guid id);
        Task<List<Feedback>> GetFeedbacks(FeedbackSearch search);
        Task<List<Feedback>> GetFeedbacksByItineraryIdAsync(Guid itineraryId);
        Task<RatingSummaryDTO> GetRatingSummaryAsync(Guid itineraryId);
        Task<Guid?> GetItineraryAgencyIdByFeedbackId(Guid feedbackId);
        Task<(double AverageRating, int TotalReviews)> GetRatingStatsByAgencyIdAsync(Guid agencyId);

    }
    public class FeedbackSearch
    {
        public string? BookingCode { get; set; }
        public FeedbackItemType? ItemType { get; set; }
        public int? Rating { get; set; }
        public string? Title { get; set; }
        public bool? IsAnonymous { get; set; }
        public FeedbackStatus? Status { get; set; }
    }
    public class RatingSummaryDTO
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingCounts { get; set; } = new();
    }
}
