using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DTO.Balance;
using DTO.Notifications;
using RestAPI.IntegrationTests.Fixture;
using Xunit;

namespace RestAPI.IntegrationTests.Tests
{
    public class NotificationControllerTests
    {
        private readonly HttpClient _httpClient;
        private Guid _correlationId;

        public NotificationControllerTests()
        {
            _httpClient = DefaultFixture.RestApi.HttpClient;
            _correlationId = Guid.NewGuid();
        }
        
        [Fact]
        public async void Get_NotificationExists_ReturnsNotification()
        {
            // Arrange
            var idempotencyKey = Guid.NewGuid();
            
            var balance = new BalanceDto
            {
                Id = idempotencyKey,
                Balance = 100
            };
            
            var notification = new NotificationInsertDto
            {
                Id = idempotencyKey,
                CorrelationId = _correlationId,
                NotificationType = balance.Id.ToString(),
                Content = JsonSerializer.Serialize(balance)
            };
            DefaultFixture.DaRunner.Run(da => da.NotificationRepository().Create(notification));
            
            // Act
            var response = await Get(idempotencyKey);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                DefaultFixture.DaRunner.Run(da => da.NotificationRepository().Get(idempotencyKey)),
                JsonSerializer.Deserialize<NotificationDto>(content));
        }
        
        [Fact]
        public async void Get_NotificationDoesntExist_ReturnsNoContent()
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
            var url = $"api/v1/notify/{id}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("CorrelationId", _correlationId.ToString());

            return await _httpClient.SendAsync(request);
        }
    }
}