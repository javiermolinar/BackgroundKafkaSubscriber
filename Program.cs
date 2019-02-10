using BackgroundKafkaSubscriber.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace BackgroundKafkaSubscriber
{
    //Todo
    /* References 
     * https://github.com/aspnet/Docs/tree/master/aspnetcore/fundamentals/host/hosted-services/samples/2.x/BackgroundTasksSample-GenericHost
     * - Inject automatically all services as https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection/blob/master/src/AutoMapper.Extensions.Microsoft.DependencyInjection/ServiceCollectionExtensions.cs
     * - Improve configuration to allow overwrite from console
     * - Allow configuring the logger for more complex scenarios (Elasticsearch)         
     */

    /*
     * How to use it
     * 
     * dotnet run --Topic test or simply dotnet run
     */

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
                   // services.Configure<ConsumerConfig>(hostContext.Configuration.GetSection("SubscriberSettings"));
                    services.Configure<ConsumerConfig>(hostContext.Configuration);
                    services.AddSingleton(hostContext.Configuration);
                    services.AddHostedService<MessageHandlerService>();                  
                });

            await builder.RunConsoleAsync();            
        }
    }
}
