using System.Data;
using DataAccess.Messages;
using DataAccess.Notifications;

namespace DataAccess.DaRunner
{
    public class DaFactory : IDaFactory
    {
        private readonly IDbConnection _connection;

        public DaFactory(IDbConnection connection)
        {
            _connection = connection;
        }

        public IMessageRepository MessageRepository()
        {
            return new MessageRepository(_connection);
        }

        public INotificationRepository NotificationRepository()
        {
            return new NotificationRepository(_connection);
        }
    }
}