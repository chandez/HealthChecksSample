using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;


using Microsoft.Extensions.Hosting;


namespace SampleApp
{
    public class Program
    {
        private static readonly Dictionary<string, Type> _scenarios;

        static Program()
        {
            _scenarios = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "basic", typeof(BasicStartup) },
                { "db", typeof(DbHealthStartup) },
                { "dbcontext", typeof(DbContextHealthStartup) },
                { "liveness", typeof(LivenessProbeStartup) },
                { "writer", typeof(CustomWriterStartup) },
                { "port", typeof(ManagementPortStartup) },
            };
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddCommandLine(args)
                .Build();

            var scenario = appConfig["scenario"] ?? "basic";

            if (!_scenarios.TryGetValue(scenario, out var startupType))
            {
                startupType = typeof(BasicStartup);
            }

            return new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddConfiguration(appConfig);
                })
                .ConfigureLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddConfiguration(appConfig);
                    builder.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(startupType);
                });
        }
    }
}
