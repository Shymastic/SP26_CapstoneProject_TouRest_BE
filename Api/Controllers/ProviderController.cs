using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/providers")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IAuthService _authService;
        private readonly IItineraryScheduleService _scheduleService;

        public ProviderController(IProviderService providerService, IAuthService authService,
            IItineraryScheduleService scheduleService)
        {
            _providerService = providerService;
            _authService = authService;
            _scheduleService = scheduleService;
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
