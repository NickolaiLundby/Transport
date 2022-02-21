using System;
using System.Data;
using Dapper.Contrib.Extensions;
using DTO.Notifications;

namespace DataAccess.Notifications
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnection _connection;

        public NotificationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Create(NotificationInsertDto dto)
        {
            var row = Inserter(dto);
            _connection.Insert(row);
        }

        public NotificationDto Get(Guid id)
        {
            var row = _connection.Get<NotificationDa>(id);
            return Converter(row);
        }

        public bool Exists(Guid id)
        {
            var row = _connection.Get<NotificationDa>(id);
            return row != default;
        }

        private NotificationDa Inserter(NotificationInsertDto dto) => new()
        {
            Id = dto.Id,
            CorrelationId = dto.CorrelationId,
            Content = dto.Content,
            NotificationType = dto.NotificationType
        };

        private NotificationDto Converter(NotificationDa da) => new()
        {
            Id = da.Id,
            CorrelationId = da.CorrelationId,
            NotificationType = da.NotificationType,
            Content = da.Content,
            CreateTime = da.CreateTime
        };
    }
}