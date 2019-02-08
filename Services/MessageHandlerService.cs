using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace BackgroundKafkaSubscriber.Services{
    public class MessageHandlerService : BackgroundService{
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var conf = new ConsumerConfig
             { 
                GroupId ="test-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoCommitIntervalMs = 5000,
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
             };           

            using (var consumer =  new ConsumerBuilder<Ignore, string>(conf).Build())
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