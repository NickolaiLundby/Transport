using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RestAPI.Hubs
{
    public class NotificationHub : Hub
    {
        public NotificationHub() { }

        public async Task Subscribe(string notificationType)
        {
            var t1 = Groups.AddToGroupAsync(Context.ConnectionId, notificationType);
            var t2 = Clients.Caller.SendAsync("Subscribed", $"Subscribed to {notificationType}.");
            await Task.WhenAll(t1, t2);
        }

        public async Task Unsubscribe(string notificationType)
        {
            var t1 = Groups.RemoveFromGroupAsync(Context.ConnectionId, notificationType);
            var t2 = Clients.Caller.SendAsync("Unsubscribed", $"Unsubscribed to {notificationType}");
            await Task.WhenAll(t1, t2);
        }
    }
}