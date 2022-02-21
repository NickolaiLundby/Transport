namespace DTO
{
    public class AppSettings
    {
        public string DatabaseConnectionString { get; set; }
        public Url Url { get; set; }
    }

    public class Url
    {
        public string BalanceService { get; set; }
        public string TransportService { get; set; }
    }
}