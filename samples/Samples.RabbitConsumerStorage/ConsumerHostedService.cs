using Alza.Client.Rabbit.Consumer;

using Samples.RabbitConsumerStorage.Contracts;

namespace Samples.RabbitConsumerStorage;

public class ConsumerHostedService(IConsumer consumer) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		consumer.Subscribe<SampleGuidMessage>();
		consumer.Subscribe<InsertMongoMessage>();
		consumer.Subscribe<InsertRedisMessage>();
    
		return consumer.ConsumeAsync(stoppingToken);
	}
}