using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DTO.Balance;
using DTO.Constants;
using DTO.Messages;
using RestAPI.IntegrationTests.Fixture;
using Xunit;

namespace RestAPI.IntegrationTests.Tests
{
    public class MessageControllerTests
    {
        private readonly HttpClient _httpClient;
        private Guid _correlationId;

        public MessageControllerTests()
        {
            _httpClient = DefaultFixture.RestApi.HttpClient;
            _correlationId = Guid.NewGuid();
        }

        [Fact]
        public async void Get_MessageExists_ReturnsMessage()
        {
            // Arrange
            var idempotencyKey = Guid.NewGuid();
            
            var balance = new BalanceUpdateDto
            {
                Id = idempotencyKey,
                Action = BalanceActions.Increment
            };
            
            var message = new MessageInsertDto
            {
                Id = idempotencyKey,
                CorrelationId = _correlationId,
                Type = MessageTypes.UpdateBalance,
                Content = JsonSerializer.Serialize(balance)
            };
            DefaultFixture.DaRunner.Run(da => da.MessageRepository().Create(message));
            
            // Act
            var response = await Get(idempotencyKey);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                DefaultFixture.DaRunner.Run(da => da.MessageRepository().Get(idempotencyKey)),
                JsonSerializer.Deserialize<MessageDto>(content));
        }
        
        [Fact]
        public async void Get_MessageDoesntExist_ReturnsNoContent()
        {
            // Arrange
            var idempotencyKey = Guid.NewGuid();

            // Act
            var response = await Get(idempotencyKey);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task<HttpResponseMessage> Get(Guid id)
        {
            var url = $"api/v1/message/{id}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("CorrelationId", _correlationId.ToString());

            return await _httpClient.SendAsync(request);
        }
    }
}