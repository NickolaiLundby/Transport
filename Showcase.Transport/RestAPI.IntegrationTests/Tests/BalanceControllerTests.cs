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
    public class BalanceControllerTests
    {
        private readonly HttpClient _httpClient;
        private Guid _correlationId;

        public BalanceControllerTests()
        {
            _httpClient = DefaultFixture.RestApi.HttpClient;
            _correlationId = Guid.NewGuid();
        }

        [Fact]
        public async void New_DoesntExists_ReturnsOk()
        {
            // Arrange
            var idempotencyKey = Guid.NewGuid();
            var balance = new BalanceDto
            {
                Id = idempotencyKey,
                Balance = 1
            };

            // Act
            var response = await Insert(idempotencyKey, balance);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(DefaultFixture.DaRunner.Run(da => da.MessageRepository().Exists(idempotencyKey)));
        }
        
        [Fact]
        public async void New_AlreadyExists_ReturnsOk()
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
            var response = await Update(idempotencyKey, balance);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> Insert(Guid id, BalanceDto balance)
        {
            var url = $"api/v1/balance/new/{id}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonSerializer.Serialize(balance), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("CorrelationId", _correlationId.ToString());

            return await _httpClient.SendAsync(request);
        }
        
        private async Task<HttpResponseMessage> Update(Guid id, BalanceUpdateDto balance)
        {
            var url = $"api/v1/balance/update/{id}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonSerializer.Serialize(balance), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("CorrelationId", _correlationId.ToString());

            return await _httpClient.SendAsync(request);
        }
    }
}