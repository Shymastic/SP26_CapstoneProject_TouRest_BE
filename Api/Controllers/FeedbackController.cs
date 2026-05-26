using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Feedback;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // GET: api/feedback/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedback(Guid id)
        {
            var result = await _feedbackService.GetFeedback(id);
            return ApiResponseFactory.Ok(result, "Get Feedback");
        }
        [HttpGet("itinerary/{itineraryId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedbacksByItinerary(Guid itineraryId)
        {
            var result = await _feedbackService.GetFeedbacksByItineraryId(itineraryId);
            return ApiResponseFactory.Ok(result);
        }
        [HttpGet("booking-itinerary/{bookingItineraryId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedbackByBookingItineraryId(Guid bookingItineraryId)
        {
            var result = await _feedbackService.GetFeedbacksByBookingItineraryId(bookingItineraryId);
            return ApiResponseFactory.Ok(result);
        }

        // POST: api/feedback
        [Authorize(Roles = "CUSTOMER")]
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateRequest request)
        {
            var result = await _feedbackService.AddFeedback(request);
            return ApiResponseFactory.Created(result, "Feedback created");
        }

        // PUT: api/feedback/{id}
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        [HttpPut("{id:guid}")]
      
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] FeedbackUpdateRequest update)
        {
            var userId = User.GetUserId();
                var result = await _feedbackService.UpdateFeedback(id, userId, update);
                return ApiResponseFactory.Ok(result);
            
        }

        // DELETE: api/feedback/{id}
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            await _feedbackService.DeleteFeedback(id);

            return ApiResponseFactory.NoContent("Feedback deleted");
        }

        //[HttpGet("search")]
        //[Authorize(Roles = "ADMIN")]
        //public async Task<IActionResult> GetFeedbacks(
        //    [FromQuery] string? bookingCode,
        //    [FromQuery] FeedbackItemType? itemType,
        //    [FromQuery] int? rating,
        //    [FromQuery] string? title,
        //    [FromQuery] bool? isAnonymous,
        //    [FromQuery] FeedbackStatus? status)
        //{
        //    var search = new FeedbackSearch
        //    {
        //        BookingCode = bookingCode,
        //        ItemType = itemType,
        //        Rating = rating,
        //        Title = title,
        //        IsAnonymous = isAnonymous,
        //        Status = status
        //    };

        //    var result = await _feedbackService.GetFeedbacks(search);
        //    return ApiResponseFactory.Ok(result);
        //}
        [HttpPut("{id:guid}/reply")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> ReplyToFeedback(Guid id, [FromBody] FeedbackReplyRequest reply)
        {
            var userId = User.GetUserId();
            await _feedbackService.CreateReplyToFeedback(id, userId, reply);
            return ApiResponseFactory.NoContent();
        }
        [HttpGet("my-booking-itinerary/{itineraryId:guid}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetMyBookingItinerary(Guid itineraryId)
        {
            var userId = User.GetUserId();
            var bookingItineraryId = await _feedbackService.GetMyBookingItineraryId(userId, itineraryId);
            if (bookingItineraryId == null)
                return NotFound(new { message = "No completed booking found for this tour." });
            return ApiResponseFactory.Ok(new { bookingItineraryId });
        }

        [HttpGet("my")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetMyFeedbacks()
        {
            var userId = User.GetUserId();
            var result = await _feedbackService.GetMyFeedbacks(userId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("itinerary/{itineraryId:guid}/summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRatingSummary(Guid itineraryId)
        {
           var summary = await _feedbackService.RatingSummary(itineraryId);
        return ApiResponseFactory.Ok(summary);
        }
    }
}
