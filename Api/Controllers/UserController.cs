using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.User;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            return ApiResponseFactory.Ok(user);
        }

        [HttpGet("")]
        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _userService.GetPagedAsync(page, pageSize, search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost("")]
        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            return ApiResponseFactory.Created(result, "User created successfully");
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> AdminUpdateUser(Guid id, [FromBody] AdminUpdateUserDTO dto)
        {
            var result = await _userService.AdminUpdateUserAsync(id, dto);
            return ApiResponseFactory.Ok(result, "User updated successfully");
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN,AGENCY")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return ApiResponseFactory.Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var userId = User.GetUserId();
            var result = await _userService.UpdateProfileAsync(userId, dto);
            return ApiResponseFactory.Ok(result, "Profile updated successfully");
        }

    }
}
