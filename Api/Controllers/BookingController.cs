using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Booking;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IProviderStaffService _staffService;

        public BookingController(IBookingService bookingService, IProviderStaffService staffService)
        {
            _bookingService = bookingService;
            _staffService   = staffService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("ADMIN");
            var booking = await _bookingService.GetBookingAsync(id, userId, isAdmin);
            return ApiResponseFactory.Ok(booking);
        }
        [HttpGet("me")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetMyBookings([FromQuery] string? status = null)
        {
            var userId = User.GetUserId();
            var result = await _bookingService.GetBookingsByUserIdAsync(userId, status);
            return ApiResponseFactory.Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public async Task<IActionResult> AddBooking([FromBody] BookingCreateRequest create)
        {
            var userId = User.GetUserId();
            var result = await _bookingService.CreateBookingAsync(create, userId);
            return ApiResponseFactory.Created(result, "Booking was created");
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingUpdateRequest update)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("ADMIN");
            var booking = await _bookingService.UpdateBookingAsync(id, userId, isAdmin, update);
            return ApiResponseFactory.Ok(booking);
        }
        [HttpGet("{bookingId:guid}/stops/{stopId:guid}/medical-results")]
        [Authorize(Roles = "CUSTOMER, ADMIN")]
        public async Task<IActionResult> GetStopMedicalResults(Guid bookingId, Guid stopId)
        {
            var result = await _staffService.GetBookingStopResultsAsync(bookingId, stopId);
            return ApiResponseFactory.Ok(result);
        }

         [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
         public async Task<IActionResult> DeleteBooking(Guid id)
            {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("ADMIN");
            await _bookingService.DeleteBookingAsync(id, userId, isAdmin);
            return ApiResponseFactory.NoContent("Booking Deleted");
        }
    }
}
