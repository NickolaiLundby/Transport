using DataAccess.Messages;
using DataAccess.Notifications;

namespace DataAccess.DaRunner
{
    public interface IDaFactory
    {
        public IMessageRepository MessageRepository();
        public INotificationRepository NotificationRepository();
    }
}