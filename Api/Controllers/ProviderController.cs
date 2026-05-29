using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;
using TouRest.Domain.DTOs;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/providers")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IAuthService _authService;
        private readonly IItineraryScheduleService _scheduleService;
        private readonly IProviderDashboardService _dashboardService;
        private readonly IProviderStaffService _staffService;
        private readonly IStorageService _storageService;

        public ProviderController(IProviderService providerService, IAuthService authService, IItineraryScheduleService scheduleService, IProviderDashboardService dashboardService, IProviderStaffService staffService, IStorageService storageService)
        {
            _providerService = providerService;
            _authService = authService;
            _scheduleService = scheduleService;
            _dashboardService = dashboardService;
            _staffService = staffService;
            _storageService = storageService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _providerService.GetAllPagedAsync(page, pageSize);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("map")]
        public async Task<IActionResult> GetMapMarkers()
        {
            var result = await _providerService.GetMapMarkersAsync();
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var result = await _providerService.GetByUserIdAsync(userId);
            if (result == null)
                return NotFound(new { message = "Provider not found." });
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _providerService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Provider not found." });
            return Ok(result);
        }

        [HttpGet("{id:guid}/detail")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _providerService.GetDetailByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Provider not found." });
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Create([FromForm] CreateProviderRequest request)
        {
            var userId = User.GetUserId();
            var result = await _providerService.CreateAsync(userId, request);
            return ApiResponseFactory.Created(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProviderRequest request)
        {
            var result = await _providerService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Provider not found." });
            }

            return Ok(result);
        }

        [HttpPost("register-request")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RegisterProviderRequest([FromBody] RegisterProviderAccountRequest request)
        {
            var currentUserId = User.GetUserId();

            await _authService.RegisterProviderAccountAsync(currentUserId, request);

            return ApiResponseFactory.Created(new { }, "Provider request registered successfully");
        }
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetStats([FromQuery] Guid providerId)
        {
            var result = await _dashboardService.GetStatsAsync(providerId);
            return Ok(result);
        }

        [HttpGet("jobs/trend")]
        public async Task<IActionResult> GetJobsTrend([FromQuery] Guid providerId, [FromQuery] int year = 2026)
        {
            var result = await _dashboardService.GetJobTrendAsync(providerId, year);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("requests/pending")]
        public async Task<IActionResult> GetPendingRequests([FromQuery] Guid providerId)
        {
            var result = await _dashboardService.GetPendingRequestsAsync(providerId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("dashboard/active-packages")]
        public async Task<IActionResult> GetActivePackages([FromQuery] Guid providerId)
        {
            var result = await _dashboardService.GetActivePackagesAsync(providerId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("dashboard/top-agencies")]
        public async Task<IActionResult> GetTopAgencies([FromQuery] Guid providerId)
        {
            var result = await _dashboardService.GetTopAgenciesAsync(providerId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("jobs/schedules")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> GetJobSchedules()
        {
            var userId = User.GetUserId();
            var provider = await _providerService.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("No provider found for the current user.");
            var result = await _scheduleService.GetByProviderIdAsync(provider.Id);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("groups")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> GetTourGroups()
        {
            var userId = User.GetUserId();
            var provider = await _providerService.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("No provider found for the current user.");
            var result = await _staffService.GetTourGroupsAsync(provider.Id);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("groups/{scheduleId:guid}/patients")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> GetPatients(Guid scheduleId)
        {
            var result = await _staffService.GetPatientsAsync(scheduleId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("groups/{scheduleId:guid}/passengers")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> GetPassengers(Guid scheduleId)
        {
            var result = await _staffService.GetPassengersAsync(scheduleId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost("groups/{scheduleId:guid}/passengers/{passengerId:guid}/results")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> SendResult(Guid scheduleId, Guid passengerId, [FromForm] string? notes, IFormFileCollection? images)
        {
            var userId = User.GetUserId();
            var provider = await _providerService.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("No provider found for the current user.");

            List<string> imageUrls = [];
            if (images != null && images.Count > 0)
                imageUrls = await _storageService.UploadManyAsync(images.ToList());

            var result = await _staffService.SendMedicalResultAsync(passengerId, scheduleId, provider.Id, notes, imageUrls);
            return ApiResponseFactory.Ok(result);
        }

        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var deleted = await _providerService.DeleteAsync(id);
        //    if (!deleted)
        //    {
        //        return NotFound(new { message = "Provider not found." });
        //    }

        //    return NoContent();
        //}
    }
}
