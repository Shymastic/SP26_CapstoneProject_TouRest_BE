namespace TouRest.Application.DTOs.Notification
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
