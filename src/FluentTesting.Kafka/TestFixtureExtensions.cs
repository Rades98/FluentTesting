using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testing.Common.Extensions;
using Testing.Common.Interfaces;

namespace Testing.Kafka
{
	/// <summary>
	/// Test fixture extensions
	/// </summary>
	public static class TestFixtureExtensions
	{
		/// <summary>
		/// Consume message from topic  consumers must be registered via <see cref="KafkaExtensions.UseKafkaAvroConsumer"/>
		/// </summary>
		/// <typeparam name="T">Message type</typeparam>
		/// <param name="fixture">Fixture</param>
		/// <param name="topic">Topic</param>
		/// <param name="timeToWait">Time to wait - default is 60 seconds</param>
		/// <returns></returns>
		public static Task<ConsumeResult<string, T>?> ConsumeKafkaMessage<T>(this ITestFixture fixture, string topic, TimeSpan? timeToWait = null)
			where T : class, ISpecificRecord
			=> fixture.ApplicationFactory.ConsumeKafkaMessage<T>(topic, timeToWait);

		public static Task PublishAvroToKafkaAndWaitForConsumptionAsync<TMessage>(this ITestFixture fixture,
			TMessage message, string topicName, string key, CancellationTokenSource cancellationTokenSource)
			where TMessage : class, ISpecificRecord
			=> fixture.PublishAvroToKafkaAndWaitForConsumptionAsync(message, topicName, key, null, cancellationTokenSource);


		/// <summary>
		/// Publish message to kafka and wait for consumption async
		/// </summary>
		public static async Task PublishAvroToKafkaAndWaitForConsumptionAsync<TMessage>(this ITestFixture fixture,
			TMessage message, string topicName, string key, List<Header>? headers, CancellationTokenSource cancellationTokenSource)
			where TMessage : class, ISpecificRecord
		{
			var config = fixture.ApplicationFactory.Services.GetRequiredService<IOptions<KafkaProducerConfig>>()!.Value;

			using var schemaRegistry = new CachedSchemaRegistryClient(config.SchemaRegistryConfig);

			using var producer = new ProducerBuilder<string, TMessage>(config)
				.SetValueSerializer(new AvroSerializer<TMessage>(schemaRegistry, config.AvroSerializerConfig).AsSyncOverAsync())
				.Build();

			var producerHeaders = new Headers();

			if (headers is not null && headers.Count != 0)
			{
				headers.ForEach(producerHeaders.Add);
			}

			await fixture.ApplicationFactory.ExecuteAndWait(producer.ProduceAsync(topicName, new Message<string, TMessage>
			{
				Key = key,
				Value = message,
				Timestamp = new(DateTimeOffset.UtcNow),
				Headers = producerHeaders
			}, cancellationTokenSource.Token), cancellationTokenSource);
		}
	}
}
