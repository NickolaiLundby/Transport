using System;

namespace DTO.Notifications
{
    public class NotificationInsertDto
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string NotificationType { get; set; }
        public string Content { get; set; }
    }
}