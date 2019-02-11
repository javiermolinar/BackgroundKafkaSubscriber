using System.Threading.Tasks;
using BackgroundKafkaSubscriber.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BackgroundKafkaSubscriber.Services
{
    public class MessageHandler1: KafkaConsumer
    {
        public override async Task OnMessageAsync(string message)
        {
            Logger.LogInformation($"Consumed message of topic 1 '{message}' at: '{message}'.");
            await Task.FromResult(0);
        }

        public MessageHandler1(IConfiguration configuration, ILogger<MessageHandler1> logger) : base( configuration, logger)
        {
            Logger.LogInformation("Starting message handler 1");
        }
    }
}
