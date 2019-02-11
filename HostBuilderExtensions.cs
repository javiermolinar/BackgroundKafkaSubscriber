using BackgroundKafkaSubscriber.Services;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackgroundKafkaSubscriber
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Add Kafka consumer to the host builder
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder AddKafkaConsumers(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddOptions(); 
                services.Configure<ConsumerConfig>(hostContext.Configuration);
                services.AddSingleton(hostContext.Configuration);
                services.AddHostedService<MessageHandlerService>();                  
            });
        }

    }
}
