using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackgroundKafkaSubscriber.Services{
    public class MessageHandlerService : BackgroundService{

        private readonly ConsumerConfig _config;
        private readonly string _topic;
        private readonly ILogger<MessageHandlerService> _logger;

        public MessageHandlerService(IOptions<ConsumerConfig> consumerConfiguration, IConfiguration configuration, ILogger<MessageHandlerService> logger)
        {
            _config = consumerConfiguration.Value;
            _topic = configuration["SubscriberSettings:Topic"];
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {             
            using (var consumer =  new ConsumerBuilder<Ignore, string>(_config).Build())
            {
                consumer.Subscribe(_topic);
                _logger.LogDebug("Broker just started");      

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(stoppingToken);
                        Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogDebug($"Error occured: {e.Error.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                await Task.FromResult(0);
                consumer.Close();
            }
        }
    }
}