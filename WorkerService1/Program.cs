using AXService.Config;
using Azure.Storage.Blobs;
using Ce.Interaction.Lib.StartupExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerService1
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                //.ReadFrom.Configuration(hostingContext.Configuration))
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();
                    services.AddBaseHttpClient();

                    services.AddSingleton(x => new BlobServiceClient(configuration["AzureBlob:ConnectionString"]));
                    services.DependencyInjectionService();
                    services.AddHostedService<BGServiceAX>();
                });
    }
}
