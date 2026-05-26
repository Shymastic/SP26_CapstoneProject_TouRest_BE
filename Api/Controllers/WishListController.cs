using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.WishList;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/wishlists")]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _wishListService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _wishListService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Wishlist item not found." });
            }

            return Ok(result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _wishListService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WishListCreateRequest request)
        {
            var result = await _wishListService.CreateAsync(request);
            return ApiResponseFactory.Created(result, "Wishlist item created successfully");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WishListUpdateRequest request)
        {
            var result = await _wishListService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Wishlist item not found." });
            }

            return ApiResponseFactory.Ok(result, "Wishlist item updated successfully");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _wishListService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Wishlist item not found." });
            }

            return ApiResponseFactory.NoContent("Wishlist item deleted successfully");
        }

        // ── Auth-based endpoints (userId from JWT) ────────────────────────────

        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> Check([FromQuery] Guid itemId)
        {
            var userId = User.GetUserId();
            var result = await _wishListService.CheckAsync(userId, itemId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMy()
        {
            var userId = User.GetUserId();
            var result = await _wishListService.GetByUserIdAsync(userId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost("me")]
        [Authorize]
        public async Task<IActionResult> AddMine([FromBody] WishListMeCreateRequest request)
        {
            var userId = User.GetUserId();
            var result = await _wishListService.AddByUserAsync(userId, request.ItemType, request.ItemId);
            return ApiResponseFactory.Created(result, "Added to wishlist");
        }

        [HttpDelete("me/item/{itemId:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveMine(Guid itemId)
        {
            var userId = User.GetUserId();
            var deleted = await _wishListService.RemoveByUserAndItemAsync(userId, itemId);
            if (!deleted)
                return NotFound(new { message = "Wishlist item not found." });
            return ApiResponseFactory.NoContent("Removed from wishlist");
        }
    }
}
