using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BackgroundKafkaSubscriber.Services{
    public class MessageHandlerService : BackgroundService{

        private readonly ConsumerConfig _config;

        public MessageHandlerService(IOptions<ConsumerConfig> consumerConfiguration)
        {
            _config = consumerConfiguration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {             
            using (var consumer =  new ConsumerBuilder<Ignore, string>(_config).Build())
            {
                consumer.Subscribe("test");        
                Console.WriteLine("Broker just started");      

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(stoppingToken);
                        Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
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