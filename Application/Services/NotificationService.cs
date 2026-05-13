using AutoMapper;
using TouRest.Application.DTOs.Notification;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<NotificationDTO>> GetMyNotificationsAsync(Guid userId)
        {
            var notifications = await _repo.GetByUserIdAsync(userId);
            return _mapper.Map<List<NotificationDTO>>(notifications);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId) =>
            await _repo.GetUnreadCountAsync(userId);

        public async Task MarkAsReadAsync(Guid notificationId) =>
            await _repo.MarkAsReadAsync(notificationId);

        public async Task MarkAllAsReadAsync(Guid userId) =>
            await _repo.MarkAllAsReadAsync(userId);
    }
}
