using Alza.BackendCore.Redis;
using Alza.BackendCore.Redis.Configuration;
using Alza.Client.Rabbit.Publisher;
using Alza.Client.Rabbit.Publisher.Configuration;
using Alza.Library.Configurations.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Samples.RabbitConsumerStorage.Data.Mongo;
using Samples.RabbitConsumerStorage.Data.Mongo.Common;
using Samples.RabbitConsumerStorage.Data.Redis;
using Samples.RabbitConsumerStorage.Services;
using Testing.Common;
using Testing.Common.Extensions;
using Testing.Common.Interfaces;
using Testing.Common.Logger;
using Testing.Mongo;
using Testing.Mongo.Seed;
using Testing.RabbitMq;
using Testing.Redis;

namespace Samples.RabbitConsumerStorage.Tests.Shared;

/// <summary>
/// Example of test fixture with custom options
/// </summary>
public class TestFixture : ITestFixture
{
	public IApplicationFactory ApplicationFactory { get; }

	public Mock<ISampleService> SampleServiceMock { get; } = new();

	public Mock<ISampleMongoRepository> SampleMongoRepoMock { get; } = new();

	public Mock<ISampleRedisRepository> SampleRedisRepoMock { get; } = new();

	public IMongoClient MongoClient { get; }

	public IRedisService Redis { get; }

	public TestFixture()
	{
		ApplicationFactory = new ApplicationFactoryBuilder<Program>()
			.RegisterServices((services, configuration) =>
				{
					services.AddLogging(builder =>
					{
						builder.ClearProviders();
						builder.AddProvider(new InMemoryLoggerProvider());
					});

					// add your mocks
					services.AddTransient(_ => SampleServiceMock.Object);
					services.AddTransient(_ => SampleMongoRepoMock.Object);
					services.AddTransient(_ => SampleRedisRepoMock.Object);

					// registering publisher allows generating messages into rabbitmq
					services.Configure<PublisherConfiguration>(options => configuration.GetSection("rabbitPublisherConfiguration").Bind(options))
						.AddBusPublisher();
				})
			.UseRabbitMq(
				// here you can register own consumer/publisher, usually can be omitted when defaults are sufficient
				(configuration, containerSettings) =>
					{
						configuration.AddObject("rabbitConsumerConfiguration", containerSettings.GenerateConsumerConfiguration());
						configuration.AddObject("rabbitPublisherConfiguration", containerSettings.GeneratePublisherConfiguration());
					},
				// here you can set your own options, usually can be omitted when defaults are sufficient
				options =>
					{
						options.QueueName = "customtestqueue";
						options.ConsumerBindings =
							[
								new()
									{
										Exchange = "customtestexchange",
										RoutingKeys =
											[
												"#.CustomRoutingKey.#"
											]
									}
							];
						options.PublisherBindings =
							[
								new()
									{
										Exchange = "customtestexchange",
										RoutingKeys =
											[
												"#.CustomRoutingKey.#"
											]
									}
							];
						options.RabbitConsumerConfigurationName = "rabbitConsumerConfiguration";
						options.RabbitPublisherConfigurationName = "rabbitPublisherConfiguration";
					}
			)
			.UseMongo(
				seed,
				(configuration, containerSettings) =>
					{
						configuration.AddObject("MongoConfigurationOptions", new MongoConfigurationOptions
						{
							Username = containerSettings.Username,
							Password = containerSettings.Password,
							DatabaseName = containerSettings.DatabaseName,
							Hosts = containerSettings.Hosts,
							Port = containerSettings.Port,
							UseTls = false
						});
					},
				// here you can set your own options, usually can be omitted when defaults are sufficient
				options =>
					{
						options.Username = "customlogin";
						options.Password = "password";
						options.DatabaseName = "TestingSamplesDatabase";
					}
				)
			.UseRedis(
				(configuration, containerSettings) =>
					{
						configuration.AddObject("RedisStorageConfiguration", new RedisStorageConfiguration { NameSpace = "TestContainers" });
						configuration.AddObject("RedisStoreConfigurationOptions", new RedisConfigurationOptions
						{
							Endpoints =
								[
									new RedisConfigurationOptions.RedisEndpoint
										{
											Url = containerSettings.Url,
											Port = containerSettings.Port,
										}
								]
						});
					},
				options =>
					{
						// here you can set your own options, usually can be omitted when defaults are sufficient
						options.Port = 6969;
					}
				)
			.Build();

		MongoClient = ApplicationFactory.Services.GetRequiredService<IMongoClient>();
		Redis = ApplicationFactory.Services.GetRequiredService<IRedisService>();

		ApplicationFactory.WaitForShutdownAsync();
	}

	readonly string seed = new SeedBuilder("TestingSamplesDatabase")
		.CreateUser("customlogin", "password")
		.InsertDocument("SampleCollection",
			"""
				{
					"text": "abcd",
					"date": new ISODate("2024-08-21T13:09:12Z")
				}
			""")
		.Build();
}