using System;
using System.Collections.Generic;
using DTO.Messages;

namespace DataAccess.Messages
{
    public interface IMessageRepository
    {
        void Create(MessageInsertDto message);
        MessageDto Get(Guid id);
        List<MessageDto> GetPending();
        bool Exists(Guid id);
        void UpdateStatus(Guid id, Guid correlationId, string status);
    }
}