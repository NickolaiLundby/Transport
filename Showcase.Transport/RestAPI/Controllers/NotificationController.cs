using System;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess.DaRunner;
using DTO.Constants;
using DTO.Messages;
using DTO.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RestAPI.Hubs;

namespace RestAPI.Controllers
{
    [Route("api/v1/notify")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly DaRunner _daRunner;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationController(DaRunner daRunner, IHubContext<NotificationHub> hub)
        {
            _daRunner = daRunner;
            _hub = hub;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<NotificationDto> Get([FromRoute] Guid id)
        {
            ActionResult<NotificationDto> result = NoContent();
            _daRunner.Run(da =>
            {
                var row = da.NotificationRepository().Get(id);
                if (row == default) return;
                
                result = row;
            });
            return result;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task NotifySubscribers([FromRoute] Guid id, [FromBody] NotificationInputDto input)
        {
            var proceed = _daRunner.Run(da =>
            {
                if (da.NotificationRepository().Exists(id)) return false;
                
                da.MessageRepository().Create(new MessageInsertDto
                {
                    Id = id,
                    CorrelationId = FetchCorrelationIdFromQuest(),
                    Type = MessageTypes.Notification,
                    Content = JsonSerializer.Serialize(input),
                    Direction = Direction.Outbound
                });
                
                da.NotificationRepository().Create(new NotificationInsertDto
                {
                    Id = id,
                    CorrelationId = FetchCorrelationIdFromQuest(),
                    Content = input.Content,
                    NotificationType = input.NotificationType
                });
                return true;
            });
            
            if (proceed)
                await _hub.Clients.Group(input.NotificationType).SendAsync("Notification", input.Content);
        }
        
        private Guid FetchCorrelationIdFromQuest()
        {
            var correlationId = Request.Headers.TryGetValue("CorrelationId", out var id)
                ? Guid.Parse(id)
                : Guid.NewGuid();
            return correlationId;
        }
    }
}