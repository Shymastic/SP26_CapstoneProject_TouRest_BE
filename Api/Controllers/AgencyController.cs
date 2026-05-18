using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;

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
        public AgencyController(ILogger<AgencyController> logger, IAgencyService agencyService,
            IAgencyUserService agencyUserService, IAuthService authService)
        {
            _logger = logger;
            _agencyService = agencyService;
            _agencyUserService = agencyUserService;
            _authService = authService;
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

        [HttpPost("register-request")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RegisterAgencyRequest([FromBody] RegisterAgencyAccountRequest request)
        {
            var currentUserId = User.GetUserId();

            await _authService.RegisterAgencyAccountAsync(currentUserId, request);

            return ApiResponseFactory.Created(new { }, "Agency request registered successfully");
        }
    }
}
