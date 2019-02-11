using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackgroundKafkaSubscriber.Services
{

    public class MessageHandlerService : BackgroundService
    {
        
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly Consumer<Ignore, string> _consumer;

        public MessageHandlerService(IOptions<ConsumerConfig> consumerConfiguration, IConfiguration configuration, ILogger<MessageHandlerService> logger)
        {         
            _logger = logger;
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfiguration.Value).Build();
            _consumer.Subscribe(configuration.GetValue("Topic", ""));
            _logger.LogInformation("Broker just started");
        }   

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        ConsumeResult<Ignore,string> cr = _consumer.Consume(stoppingToken);
                        _logger.LogInformation($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogInformation($"Error occured: {e.Error.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }, stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Broker is stopped");           
            _consumer.Close();
            _consumer.Dispose();
            return Task.CompletedTask;
        }        
    }
}