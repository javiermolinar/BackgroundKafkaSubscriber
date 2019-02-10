using BackgroundKafkaSubscriber.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace BackgroundKafkaSubscriber
{
    public class Program
    {
        public static async Task Main(string[] args)
        {          
            var builder = new HostBuilder()
                .ConfigureLogging((hostContext, config) =>
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();                    
                    services.Configure<ConsumerConfig>(hostContext.Configuration.GetSection("SubscriberSettings"));
                    services.AddSingleton(hostContext.Configuration);
                    services.AddHostedService<MessageHandlerService>();                  
                });

            await builder.RunConsoleAsync();
        }
    }
}
