using AutoMapper;
using Org.BouncyCastle.Crypto.Fpe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        private readonly IAgencyUserRepository _agencyUserRepository;
        private readonly IMapper _mapper;
        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper, 
            IBookingItineraryRepository bookingItineraryRepository, IAgencyUserRepository agencyUserRepository)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
            _bookingItineraryRepository = bookingItineraryRepository;
            _agencyUserRepository = agencyUserRepository;
        }
        public async Task<List<FeedbackDTO>> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId)
        {
            var list = await _feedbackRepository.GetFeedbacksByBookingItineraryIdAsync(bookingItineraryId);
            return _mapper.Map<List<FeedbackDTO>>(list);
        }
        public async Task<List<FeedbackDTO>> GetFeedbacksByItineraryId(Guid itineraryId)
        {
            var list = await _feedbackRepository.GetFeedbacksByItineraryIdAsync(itineraryId);
            return _mapper.Map<List<FeedbackDTO>>(list);
        }
        public async Task<FeedbackDTO?> GetFeedback(Guid id)
        {
            var feedback = await _feedbackRepository.GetFeedback(id);
            return _mapper.Map<FeedbackDTO>(feedback);
        }
        public async Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search)
        {
            var list = await _feedbackRepository.GetFeedbacks(search);
            return _mapper.Map<List<FeedbackDTO>>(list);
        }
        public async Task<FeedbackDTO> AddFeedback(FeedbackCreateRequest create)
        {
            var bookingItinenary = await _bookingItineraryRepository.GetByIdAsync(create.BookingItineraryId);
            if (bookingItinenary == null)
            {
                throw new KeyNotFoundException("Booking Itinenary not found");
            }
            if (bookingItinenary.Status != BookingItineraryStatus.Completed)
                throw new InvalidOperationException("Can only leave feedback after tour is completed");
            var feedback = _mapper.Map<Feedback>(create);
            var result = await _feedbackRepository.CreateAsync(feedback);
            return _mapper.Map<FeedbackDTO>(result);
        }
        public async Task DeleteFeedback(Guid id)
        {
            var feedback = await _feedbackRepository.GetFeedback(id);
            if (feedback == null)
                throw new KeyNotFoundException("Feedback not found");
            feedback.Status = FeedbackStatus.Archived;
            await _feedbackRepository.UpdateAsync(feedback);
        }
        public async Task<FeedbackDTO> UpdateFeedback(Guid id, Guid userId, FeedbackUpdateRequest update)
        {
            var feedback = await _feedbackRepository.GetFeedback(id);
            if(feedback == null)
            {
                throw new KeyNotFoundException("Feedback not found");
            }
            if (feedback.BookingItinerary.Booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own feedback");
            _mapper.Map(update, feedback);
            var result = await _feedbackRepository.UpdateAsync(feedback);
            return _mapper.Map<FeedbackDTO>(result);
        }
        public async Task CreateReplyToFeedback(Guid id, Guid staffId, FeedbackReplyRequest replyRequest) 
        {
            var feedback = await _feedbackRepository.GetFeedback(id);
            if(feedback == null)
            {
                throw new KeyNotFoundException("Feedback not found");
            }

            var agencyUser = await _agencyUserRepository.GetAgencyUserByUserId(staffId);
            if (agencyUser == null)
                throw new UnauthorizedAccessException("User is not part of any agency");

            var itineraryAgencyId = await _feedbackRepository.GetItineraryAgencyIdByFeedbackId(id);
            if (agencyUser.AgencyId != itineraryAgencyId)
                throw new UnauthorizedAccessException("You can only reply to feedbacks from your agency");
            feedback.AgencyReply = replyRequest.Message;
            feedback.RepliedByUserId = staffId;
            feedback.RepliedAt = DateTime.UtcNow;
            feedback.UpdatedAt = DateTime.UtcNow;
            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task<RatingSummaryDTO> RatingSummary(Guid itineraryId)
        {
            var feedback = await _feedbackRepository.GetFeedbacksByItineraryIdAsync(itineraryId);
            if(feedback == null)
            {
                throw new KeyNotFoundException("Itinerary not found");
            }
            var summary = await _feedbackRepository.GetRatingSummaryAsync(itineraryId);
            return summary;
        }

        public async Task<Guid?> GetMyBookingItineraryId(Guid userId, Guid itineraryId)
        {
            var bi = await _bookingItineraryRepository.GetCompletedByUserAndItinerary(userId, itineraryId);
            return bi?.Id;
        }

        public async Task<List<FeedbackDTO>> GetMyFeedbacks(Guid userId)
        {
            var feedbacks = await _feedbackRepository.GetMyFeedbacksAsync(userId);
            return _mapper.Map<List<FeedbackDTO>>(feedbacks);
        }
    }
    }
