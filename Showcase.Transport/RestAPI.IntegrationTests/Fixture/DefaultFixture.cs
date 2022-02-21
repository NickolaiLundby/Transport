using System;
using DataAccess.DaRunner;
using DTO;
using Microsoft.Extensions.Configuration;

namespace RestAPI.IntegrationTests.Fixture
{
    public class DefaultFixture
    {
        public static DaRunner DaRunner { get; } = CreateDaRunner();
        public static WebServerFixture RestApi { get; } = new WebServerFixture();
        public static WireMockFixture WireMock { get; } = new WireMockFixture();
        public static AppSettings Config { get; set; } = GetSettings();

        static DefaultFixture(){}

        private static AppSettings GetSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Config/appsettings.json", false)
                .Build();
            return configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        private static DaRunner CreateDaRunner()
        {
            var connString = GetSettings().DatabaseConnectionString;
            return new DaRunner(connString);
        }
    }
}