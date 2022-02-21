using System;
using DTO.Notifications;

namespace DataAccess.Notifications
{
    public interface INotificationRepository
    {
        void Create(NotificationInsertDto notification);
        NotificationDto Get(Guid id);
        bool Exists(Guid id);
    }
}