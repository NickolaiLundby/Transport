using System;
using Dapper.Contrib.Extensions;

namespace DataAccess.Notifications
{
    [Table("Notifications")]
    public class NotificationDa
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string NotificationType { get; set; }
        public string Content { get; set; }
        [Write(false)]
        public DateTimeOffset CreateTime { get; set; }
    }
}