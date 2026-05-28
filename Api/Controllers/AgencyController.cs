using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;

namespace TouRest.Api.Controllers
{
    [Route("api/agencies")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly ILogger<AgencyController> _logger;
        private readonly IAgencyService _agencyService;
        private readonly IAgencyUserService _agencyUserService;
        private readonly IAuthService _authService;
        private readonly IAgencyDashboardService _dashboardService;
        public AgencyController(ILogger<AgencyController> logger, IAgencyService agencyService,
            IAgencyUserService agencyUserService, IAuthService authService, IAgencyDashboardService agencyDashboardService)
        {
            _logger = logger;
            _agencyService = agencyService;
            _agencyUserService = agencyUserService;
            _authService = authService;
            _dashboardService = agencyDashboardService;
        }
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllAgencies([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _agencyService.GetAllAsync(page, pageSize);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAgencyById(Guid id)
        {
            var agency = await _agencyService.GetAgencyById(id);
            if (agency == null)
                return NotFound();
            return ApiResponseFactory.Ok(agency);
        }

        [HttpGet("{id:guid}/detail")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _agencyService.GetDetailByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Agency not found." });
            return ApiResponseFactory.Ok(result);
        }
        [HttpGet("user-list")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> GetAgencyUsers(Guid agencyId)
        {
            var users = await _agencyUserService.GetAgencyUsers(agencyId);
            return ApiResponseFactory.Ok(users);
        }
        [HttpGet("me")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetMyAgency()
        {
            var user = User.GetUserId();
            var result = await _agencyService.GetMyAgency(user);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("{id:guid}/guides")]
        public async Task<IActionResult> GetAgencyGuides(Guid id)
        {
            var result = await _agencyUserService.GetGuidesAsync(id);
            return ApiResponseFactory.Ok(result);
        }
        // Dashboard endpoints
        [HttpGet("dashboard/stats")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var agencyId = await GetAgencyId();
            var result = await _dashboardService.GetStatsAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("dashboard/schedules/upcoming")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetUpcomingSchedules()
        {
            var agencyId = await GetAgencyId();
            var result = await _dashboardService.GetUpcomingSchedulesAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("dashboard/bookings/recent")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetRecentBookings()
        {
            var agencyId = await GetAgencyId();
            var result = await _dashboardService.GetRecentBookingsAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("dashboard/guides/workload")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetGuideWorkload()
        {
            var agencyId = await GetAgencyId();
            var result = await _dashboardService.GetGuideWorkloadAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }
        // Agency's own guide list
        [HttpGet("guides")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetMyGuides()
        {
            var agencyId = await GetAgencyId();
            var result = await _agencyUserService.GetGuidesAsync(agencyId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost("{agencyId:guid}/add-user")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> AddUserToAgency(Guid agencyId, [FromBody] AddUserIntoAgencyRequest request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is adding user {AddedUserId} to agency {AgencyId}", user, request.AddUserId, agencyId);
            await _agencyUserService.AddUserToAgencyAsync(agencyId, request.AddUserId, request.Role);
            return ApiResponseFactory.Ok(new { }, "User added to agency");
        }
        [HttpPost("{agencyId:guid}/remove-user")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> RemoveUserFromAgency(Guid agencyId, [FromBody] RemoveUserRequest request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is removing user {RemovedUserId} from agency {AgencyId}", user, request.UserId, agencyId);
            await _agencyUserService.RemoveUserFromAgencyAsync(agencyId, request.UserId);
            return ApiResponseFactory.Ok(new { }, "User removed from agency");
        }
        
        [HttpPut("{agencyId:guid}")]
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateAgency([FromForm] AgencyCreateRequestDTO request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is creating an agency with name {AgencyName}", user, request.Name);
            var result = await _agencyService.AddAgency(user, request);
            return ApiResponseFactory.Created(result, "Agency created. Please wait for Administrator to approve");
        }
        [HttpPut]
        [Authorize(Roles = "CUSTOMER, AGENCY, ADMIN")]
        public async Task<IActionResult> UpdateAgency(Guid agencyId, [FromBody] AgencyUpdateRequestDTO request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is updating agency {AgencyId}", user, agencyId);
            var result = await _agencyService.UpdateAgency(agencyId, request);
            return ApiResponseFactory.Ok(result, "Agency updated");
        }
        [HttpDelete("{agencyId:guid}")]
        [Authorize(Roles = "AGENCY, ADMIN")]
        public async Task<IActionResult> DeactivateAgency(Guid agencyId)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is deactivating agency {AgencyId}", user, agencyId);
            await _agencyService.DeactivateAgency(agencyId);
            return ApiResponseFactory.Ok(new { }, "Agency deactivated");
        }
        [HttpPost("register-request")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RegisterAgencyRequest([FromBody] RegisterAgencyAccountRequest request)
        {
            var currentUserId = User.GetUserId();

            await _authService.RegisterAgencyAccountAsync(currentUserId, request);

            return ApiResponseFactory.Created(new { }, "Agency request registered successfully");
        }

        private async Task<Guid> GetAgencyId()
        {
            var userId = User.GetUserId();
            var agencyUser = await _agencyUserService.GetAgencyUserByUserId(userId);
            if (agencyUser == null)
                throw new UnauthorizedAccessException("User is not part of any agency");
            return agencyUser.AgencyId;
        }
    }
}
