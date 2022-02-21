using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RestAPI
{
    public class Program
    {
        public static void Main()
        {
            CreateHostBuilder(typeof(Program).Assembly).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(Assembly assembly) =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}