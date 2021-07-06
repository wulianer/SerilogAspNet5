using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeriLogASPNet5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* Method 1: Config the Serilog Logger with functions 
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            //Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("../logs/myapplog.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
            */

            // Method 2: Config the Serilog Logger from Configure file
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();


            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();                
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");                
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
