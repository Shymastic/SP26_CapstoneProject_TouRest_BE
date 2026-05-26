using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.ItineraryActivity;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/itinerary-activities")]
    [ApiController]
    public class ItineraryActivityController : ControllerBase
    {
        private readonly ILogger<ItineraryActivityController> _logger;
        private readonly IItineraryActivityService _itineraryActivityService;
        private readonly IItineraryStopService _itineraryStopService;
        public ItineraryActivityController(ILogger<ItineraryActivityController> logger ,IItineraryActivityService itineraryActivityService, IItineraryStopService itineraryStopService)
        {
            _logger = logger;
            _itineraryActivityService = itineraryActivityService;
            _itineraryStopService = itineraryStopService;
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetItineraryActivityById(Guid id)
        {
            var activity = await _itineraryActivityService.GetItineraryActivity(id);
            if (activity == null)
            {
                return ApiResponseFactory.NoContent("Itinerary activity not found");
            }
            return ApiResponseFactory.Ok(activity);

        }
        [HttpGet("stop/{stopId:guid}")]
        public async Task<IActionResult> GetItineraryActivitiesByStopId(Guid stopId)
        {
            var activities = await _itineraryActivityService.GetActivitiesByItineraryStopId(stopId);
            return ApiResponseFactory.Ok(activities);
        }
        [HttpPost("stop/{stopId:guid}")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> AddItineraryActivity(Guid stopId, [FromBody] ItineraryActivityCreateRequest create)
        {
            var agencyId = User.GetUserId();
            _logger.LogInformation("Adding itinerary activity for stop {StopId} by agency {AgencyId}", stopId, agencyId);
            var stop = await _itineraryStopService.GetItineraryStopById(stopId);
            if (stop == null)
            {
                return ApiResponseFactory.NoContent("Itinerary stop not found");
            }
            var result = await _itineraryActivityService.AddItineraryActivity(create, stopId);
            return ApiResponseFactory.Created(result);
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> DeleteItineraryActivity(Guid id)
        {
            var agencyId = User.GetUserId();
            _logger.LogInformation("Deleting itinerary activity {ActivityId} by agency {AgencyId}", id, agencyId);
            var activity = await _itineraryActivityService.GetItineraryActivity(id);
            if (activity == null)
            {
                return ApiResponseFactory.NoContent("Itinerary activity not found");
            }
            var result = await _itineraryActivityService.DeleteItineraryActivity(id);
            if (!result)
            {
                return ApiResponseFactory.NoContent("Failed to delete itinerary activity");
            }
            return ApiResponseFactory.Ok("Itinerary activity deleted successfully");
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> UpdateItineraryActivity(Guid id, [FromBody] ItineraryActivityUpdateRequest update)
        {
            var agencyId = User.GetUserId();
            _logger.LogInformation("Updating itinerary activity {ActivityId} by agency {AgencyId}", id, agencyId);
            var existing = await _itineraryActivityService.GetItineraryActivity(id);
            if (existing == null)
            {
                return ApiResponseFactory.NoContent("Itinerary activity not found");
            }
            var result = await _itineraryActivityService.UpdateItineraryActivity(id, update);
            return ApiResponseFactory.Ok(result, "Itinerary activity updated successfully");
        }
    }
    }
