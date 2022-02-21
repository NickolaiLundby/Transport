using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace RestAPI.IntegrationTests.Fixture
{
    public class WebServerFixture
    {
        public HttpClient HttpClient { get; set; }

        public WebServerFixture()
        {
            HttpClient = CreateTestServer();
        }

        private static HttpClient CreateTestServer()
        {
            var host = Program
                .CreateHostBuilder(typeof(Program).Assembly)
                .ConfigureWebHost(c => c.UseTestServer())
                .Build();
            host.Start();

            return host.GetTestClient();
        }
    }
}