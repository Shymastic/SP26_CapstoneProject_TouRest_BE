using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.DTOs.ItineraryStop;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/itineraries")]
    [ApiController]
    public class ItineraryController : ControllerBase
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
        private readonly ILogger<ItineraryController> _logger;
        private readonly IItineraryService _itineraryService;
        private readonly IAgencyService _agencyService;
        private readonly IItineraryScheduleService _scheduleService;
        private readonly IItineraryStopRepository _stopRepository;

        public ItineraryController(IItineraryService itineraryService, IAgencyService agencyService,
            IItineraryScheduleService scheduleService, IItineraryStopRepository stopRepository,
            ILogger<ItineraryController> logger)
        {
            _itineraryService = itineraryService;
            _agencyService = agencyService;
            _scheduleService = scheduleService;
            _stopRepository = stopRepository;
            _logger = logger;
        }

        private async Task<Guid> ResolveAgencyIdAsync()
        {
            var userId = User.GetUserId();
            var agency = await _agencyService.GetMyAgency(userId)
                ?? throw new KeyNotFoundException("No agency found for the current user.");
            return agency.Id;
        }
        // API endpoints for Itinerary
        [HttpGet]
        public async Task<IActionResult> GetItineraries([FromQuery] ItinerarySearch search)
        {
            var result = await _itineraryService.GetItineraries(search);
            return ApiResponseFactory.Ok(result);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetItineraryById(Guid id)
        {
            var itinerary = await _itineraryService.GetItineraryById(id);
            if (itinerary == null)
                return NotFound();
            return ApiResponseFactory.Ok(itinerary);
        }
        [HttpGet("my")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetMyItineraries()
        {
            var agencyId = await ResolveAgencyIdAsync();
            var result = await _itineraryService.GetMyItinerariesAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost("full")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> CreateFull([FromForm] ItineraryFullCreateRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.StopsJson))
                request.Stops = JsonSerializer.Deserialize<List<ItineraryStopInlineRequest>>(
                    request.StopsJson, _jsonOptions) ?? [];

            var agencyId = await ResolveAgencyIdAsync();
            var result = await _itineraryService.CreateFullAsync(agencyId, request);
            return ApiResponseFactory.Created(result, "Itinerary created");
        }

        [HttpPost]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> AddItinerary([FromBody] ItineraryCreateRequest create)
        {
            var agencyId = await ResolveAgencyIdAsync();
            _logger.LogInformation("Adding itinerary for agency {AgencyId}", agencyId);
            var result = await _itineraryService.AddItinerary(agencyId, create);
            return ApiResponseFactory.Created<ItineraryDTO>(result, "Itinerary created");
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> UpdateItinerary(Guid id, [FromBody] ItineraryUpdateRequest update)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Updating itinerary {ItineraryId} by user {userId}", id, userId);
            var result = await _itineraryService.UpdateItinerary(id, update);
            return ApiResponseFactory.Ok(result,"itinerary updated");
        }
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest status)
        {
            try
            {
                var userId = User.GetUserId();
                _logger.LogInformation("Updating itinerary {ItineraryId} status by user {userId}", id, userId);
                var result = await _itineraryService.UpdateItineraryStatus(id, status);
                return ApiResponseFactory.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> DeleteItinerary(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Deleting itinerary {ItineraryId} by user {userId}", id, userId);
            try
            {
                await _itineraryService.DeleteItinerary(id);
                return ApiResponseFactory.NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // ── Schedule endpoints ────────────────────────────────────────────────
        [HttpGet("schedules/agency")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetAgencySchedules()
        {
            var agencyId = await ResolveAgencyIdAsync();
            var result = await _scheduleService.GetByAgencyIdAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("schedules/my-guide")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetMyGuideSchedules()
        {
            var userId = User.GetUserId();
            var result = await _scheduleService.GetByGuideIdAsync(userId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("{id:guid}/schedules")]
        public async Task<IActionResult> GetSchedules(Guid id)
        {
            var result = await _scheduleService.GetByItineraryIdAsync(id);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost("{id:guid}/schedules")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> AddSchedule(Guid id, [FromBody] ItineraryScheduleCreateRequest request)
        {
            try
            {
                var result = await _scheduleService.AddAsync(id, request);
                return ApiResponseFactory.Created(result, "Schedule added");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}/schedules/{scheduleId:guid}")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> DeleteSchedule(Guid id, Guid scheduleId)
        {
            var result = await _scheduleService.DeleteAsync(scheduleId);
            if (result)
                return ApiResponseFactory.NoContent();
            else
                return NotFound();
        }

        // ── Provider endpoints ────────────────────────────────────────────────
        [HttpGet("{id:guid}/providers")]
        public async Task<IActionResult> GetItineraryProviders(Guid id)
        {
            var result = await _itineraryService.GetProvidersInItineraryAsync(id);
            return ApiResponseFactory.Ok(result);
        }

        // ── Stop endpoints ────────────────────────────────────────────────────
        [HttpGet("{id:guid}/stops")]
        public async Task<IActionResult> GetStopsWithActivities(Guid id)
        {
            var stops = await _stopRepository.GetWithActivitiesByItineraryIdAsync(id);
            var result = stops.Select(s => new ItineraryStopWithActivitiesDTO
            {
                Id = s.Id,
                StopOrder = s.StopOrder,
                Name = s.Name,
                Address = s.Address,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                ProviderId = s.ProviderId,
                Activities = s.Activities.Select(a => new StopActivityDTO
                {
                    Id = a.Id,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service?.Name,
                    CustomName = a.CustomName,
                    ServiceDescription = a.Service?.Description,
                    ActivityOrder = a.ActivityOrder,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Price = a.Price,
                    Note = a.Note,
                }).ToList()
            }).ToList();
            return ApiResponseFactory.Ok(result);
        }
    }
}
