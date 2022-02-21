using System.Reflection;
using CommandLine;
using DbUp;

namespace DbSetup
{
    public class Options
    {
        [Option('s', "server", Required = false, HelpText = "sql server", Default = "")]
        public string Server { get; set; }
    
        [Option('d', "database", Required = false, HelpText = "database name", Default = "")]
        public string Database { get; set; }
    
        [Option('p', "password", Required = false, HelpText = "password", Default = "")]
        public string Password { get; set; }
    
        [Option('u', "user", Required = false, HelpText = "user", Default = "")]
        public string User { get; set; }
    }

    internal class Program
    {
        private static int Main(string[] args)
        {
            var result = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                options.Server = !string.IsNullOrWhiteSpace(options.Server) ? options.Server : "(localdb)\\MSSQLLocalDB ";
                options.Database = !string.IsNullOrWhiteSpace(options.Database) ? options.Database : "Showcase_Transport";

                var connectionString = string.IsNullOrWhiteSpace(options.User)
                    ? $"Server={options.Server};Database={options.Database};Trusted_Connection=Yes;"
                    : $"Server={options.Server};Database={options.Database};User Id={options.User};Password={options.Password}";

                EnsureDatabase.For.SqlDatabase(connectionString);
                var upgradeEngine = DeployChanges.To
                    .SqlDatabase(connectionString)
                    .JournalToSqlTable("dbo", "SchemaVersions")
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                result = upgradeEngine.PerformUpgrade().Successful;
            });

            return result ? 0 : -1;
        }
    }
}