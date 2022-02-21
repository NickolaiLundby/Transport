using System;

namespace DTO.Messages
{
    public class MessageInsertDto
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; }
        public string Content { get; set; }
    }
}