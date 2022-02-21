using System;
using System.Text.Json;
using DataAccess.DaRunner;
using DTO.Balance;
using DTO.Constants;
using DTO.Messages;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers
{
    [Route("api/v1/balance")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly DaRunner _daRunner;

        public BalanceController(DaRunner daRunner)
        {
            _daRunner = daRunner;
        }
        
        [HttpPut]
        [Route("new/{idempotencyKey}")]
        public ActionResult Create([FromRoute] Guid idempotencyKey, [FromBody] BalanceDto dto)
        {
            var correlationId = FetchCorrelationIdFromQuest();

            _daRunner.Run(da =>
            {
                if (da.MessageRepository().Exists(idempotencyKey)) return;

                da.MessageRepository().Create(new MessageInsertDto
                {
                    Id = idempotencyKey,
                    CorrelationId = correlationId,
                    Type = MessageTypes.CreateBalance,
                    Direction = Direction.Inbound,
                    Content = JsonSerializer.Serialize(dto)
                });
            });

            return Ok();
        }
        
        [HttpPut]
        [Route("update/{idempotencyKey}")]
        public ActionResult Update([FromRoute] Guid idempotencyKey, [FromBody] BalanceUpdateDto dto)
        {
            var correlationId = FetchCorrelationIdFromQuest();

            _daRunner.Run(da =>
            {
                if (da.MessageRepository().Exists(idempotencyKey)) return;

                da.MessageRepository().Create(new MessageInsertDto
                {
                    Id = idempotencyKey,
                    CorrelationId = correlationId,
                    Type = MessageTypes.UpdateBalance,
                    Direction = Direction.Inbound,
                    Content = JsonSerializer.Serialize(dto)
                });
            });

            return Ok();
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