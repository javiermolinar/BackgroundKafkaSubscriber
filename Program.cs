using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using BackgroundKafkaSubscriber.Services;
using Confluent.Kafka;

namespace BackgroundKafkaSubscriber
{
    class Program
    {
        public static async Task Main(string[] args)
        {   
            var host = new HostBuilder().ConfigureLogging((hostContext, config) =>
                {
                    config.AddConsole();
                    config.AddDebug();
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddCommandLine(args);                    
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton(hostContext.Configuration);
                    services.Configure<ConsumerConfig>(hostContext.Configuration.GetSection("SubscriberSettings"));                
                    services.AddSingleton<IHostedService, MessageHandlerService>();
                })
                .Build();

            using (host)
            {
                Console.WriteLine("Starting the service");
                // Start the host
                await host.StartAsync();             
                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
                Console.WriteLine("Shutdown has been triggered");                 
            }
        }
    }
}
