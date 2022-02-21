using System;

namespace DTO.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string NotificationType { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreateTime { get; set; }
    }
}