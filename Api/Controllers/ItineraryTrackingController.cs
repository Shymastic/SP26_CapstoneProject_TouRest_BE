using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Application.DTOs.Tracking;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/itinerary-tracking")]
    [Authorize]
    public class ItineraryTrackingController : ControllerBase
    {
        private readonly IItineraryTrackingService _service;

        public ItineraryTrackingController(IItineraryTrackingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByScheduleId([FromQuery] Guid scheduleId)
        {
            var result = await _service.GetByScheduleIdAsync(scheduleId);
            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Track([FromBody] TrackRequest request)
        {
            var result = await _service.TrackAsync(request);
            return Ok(new { success = true, data = result });
        }

        [HttpDelete]
        public async Task<IActionResult> Untrack(
            [FromQuery] Guid scheduleId,
            [FromQuery] Guid trackingId,
            [FromQuery] ItineraryTrackingType type)
        {
            var deleted = await _service.UntrackAsync(scheduleId, trackingId, type);
            if (!deleted) return NotFound(new { success = false, message = "Tracking record not found" });
            return Ok(new { success = true });
        }
    }
}
