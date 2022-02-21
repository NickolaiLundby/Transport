using System;
using DataAccess.DaRunner;
using DTO.Messages;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers
{
    [Route("api/v1/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly DaRunner _daRunner;

        public MessageController(DaRunner daRunner)
        {
            _daRunner = daRunner;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<MessageDto> GetById([FromRoute] Guid id)
        {
            ActionResult<MessageDto> result = NoContent();
            _daRunner.Run(da =>
            {
                var row = da.MessageRepository().Get(id);
                if (row == default) return;
                
                result = row;
            });
            return result;
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