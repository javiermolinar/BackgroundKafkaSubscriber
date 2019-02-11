using System.Threading.Tasks;
using BackgroundKafkaSubscriber.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BackgroundKafkaSubscriber.Services
{

    public class MessageHandler2 : KafkaConsumer
    {
        public override async Task OnMessageAsync(string message)
        {
            Logger.LogInformation($"Consumed message of topic 2: '{message}' at: '{message}'.");
            await Task.FromResult(0);
        }

        public MessageHandler2(IConfiguration configuration, ILogger<MessageHandler2> logger) : base(configuration, logger,"test2")
        {
            Logger.LogInformation("Starting message handler 2");
        }
    }
}