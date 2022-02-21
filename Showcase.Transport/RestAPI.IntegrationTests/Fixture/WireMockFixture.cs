using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace RestAPI.IntegrationTests.Fixture
{
    public class WireMockFixture
    {
        public readonly WireMockServer WireMockServer = WireMockServer.Start("http://localhost:9632/");

        public WireMockFixture()
        {
            MockBalanceService();
        }

        private void MockBalanceService()
        {
            WireMockServer
                .Given(Request.Create()
                    .WithPath("/balance/new/*")
                    .UsingPut())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));
            
            WireMockServer
                .Given(Request.Create()
                    .WithPath("/balance/update/*")
                    .UsingPut())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));
        }
    }
}