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

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.GetUserId();
            var result = await _bookingService.GetBookingsByUserIdAsync(userId);
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
