using TouRest.Application.DTOs.Notification;

namespace TouRest.Application.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetMyNotificationsAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
