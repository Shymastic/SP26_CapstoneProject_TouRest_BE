using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search);
        Task<List<FeedbackDTO>> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId);
        Task<List<FeedbackDTO>> GetFeedbacksByItineraryId(Guid itineraryId);
        Task<FeedbackDTO?> GetFeedback(Guid id);
        Task<FeedbackDTO> AddFeedback(FeedbackCreateRequest create);
        Task<FeedbackDTO> UpdateFeedback(Guid id, Guid userId, FeedbackUpdateRequest update);
        Task CreateReplyToFeedback(Guid id, Guid staffId, FeedbackReplyRequest replyRequest);
        Task DeleteFeedback(Guid id);
        Task<RatingSummaryDTO> RatingSummary(Guid itineraryId);
        Task<Guid?> GetMyBookingItineraryId(Guid userId, Guid itineraryId);
        Task<List<FeedbackDTO>> GetMyFeedbacks(Guid userId);
    }
}
