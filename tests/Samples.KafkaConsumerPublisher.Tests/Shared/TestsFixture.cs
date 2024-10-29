using Alza.Client.Kafka.Common.Configuration.Consumer;
using Alza.Client.Kafka.Publisher;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Samples.KafkaConsumerPublisher.Services;
using Testing.Common;
using Testing.Common.Extensions;
using Testing.Common.Interfaces;
using Testing.Kafka;

namespace Samples.KafkaConsumerPublisher.Tests.Shared
{
	public class TestsFixture : ITestFixture
	{
		public IApplicationFactory ApplicationFactory { get; }

		public readonly Mock<IAvroHandlingService> ServiceMock = new();

		public TestsFixture()
		{
			ApplicationFactory = new ApplicationFactoryBuilder<Program>()
				.RegisterServices((services, configuration) =>
				{
					services.AddTransient(_ => ServiceMock.Object);

					//Add kafka publisher
					services.ConfigureKafkaPublisher(configuration);
				})
				.UseKafka(opts =>
				{
					opts.UseKafkaConsumption = true;
					opts.TopicNames = [
						"incoming.topic",
					];

					opts.TestConsumerTopicNames = ["outgoing.topic"];

				}, (config, kafkaOpts) =>
				{
					config.AddObject("KafkaConsumerConfig", new
					{
						BootstrapServers = kafkaOpts.BootstrapServer,
						SchemaRegistryConfig = new
						{
							Url = kafkaOpts.SchemaRegistryUrl,
						},
						GroupId = "testing",
						AutoOffsetReset = AutoOffsetReset.Earliest,
						Topics = kafkaOpts.KafkaOptions.TopicNames.Where(topic => !topic.Contains("DLT")).Select(name => new TopicConverterPair() { TopicName = name }).ToList(),
						StatisticsIntervalMs = 0,
						RetryTopics = kafkaOpts.KafkaOptions.TopicNames.Select(topic => new
						{
							RetryCount = 1,
							RetryTopic = topic
						}),
					});

					config.AddObject("KafkaProducerConfig", new
					{
						BootstrapServers = kafkaOpts.BootstrapServer,
						SchemaRegistryConfig = new
						{
							Url = kafkaOpts.SchemaRegistryUrl,
						},
						AvroSerializerConfig = new
						{
							UseLatestVersion = false,
							AutoRegisterSchemas = true
						}
					});
				})
				.Build()
				.UseKafkaAvroConsumer();

			ApplicationFactory.WaitForShutdownAsync();
		}
	}
}
