using System;
using Dapper.Contrib.Extensions;

namespace DataAccess.Messages
{
    [Table("Messages")]
    public class MessageDa
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Status { get; set; }
        public string Direction { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        [Write(false)]
        public DateTimeOffset CreateTime { get; set; }
        [Write(false)]
        public DateTimeOffset UpdateTime { get; set; }
    }
}