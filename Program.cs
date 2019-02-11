using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using BackgroundKafkaSubscriber.Infrastructure;
using Microsoft.Extensions.Logging;


namespace BackgroundKafkaSubscriber
{
    //Todo
    /* References 
     * https://github.com/aspnet/Docs/tree/master/aspnetcore/fundamentals/host/hosted-services/samples/2.x/BackgroundTasksSample-GenericHost
     * - Inject automatically all services as https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection/blob/master/src/AutoMapper.Extensions.Microsoft.DependencyInjection/ServiceCollectionExtensions.cs
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
                    config.AddJsonFile("appsettings.json", true);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",true);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions(); 
                })
                .AddKafkaConsumers();

            await builder.RunConsoleAsync();            
        }
    }
}
