using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.GetUserId();
            var result = await _service.GetMyNotificationsAsync(userId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.GetUserId();
            var count = await _service.GetUnreadCountAsync(userId);
            return ApiResponseFactory.Ok(count);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _service.MarkAsReadAsync(id);
            return ApiResponseFactory.Ok("Marked as read");
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.GetUserId();
            await _service.MarkAllAsReadAsync(userId);
            return ApiResponseFactory.Ok("All marked as read");
        }
    }
}
