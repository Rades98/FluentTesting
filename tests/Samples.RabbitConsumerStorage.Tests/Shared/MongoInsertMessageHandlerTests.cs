using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Data.Mongo;
using Samples.RabbitConsumerStorage.Tests.Shared;
using Testing.Common.Extensions;
using Testing.RabbitMq;
using Xunit;

namespace Samples.RabbitConsumerStorage.Tests;

[Collection("RabbitConsumerWithMongoAndRedis")]
public class MongoInsertMessageHandlerTests(TestFixture fixture)
{
	[Fact]
	public async Task HandlerReturns_True()
	{
		// Arrange
		var cts = fixture.GetCtsWithTimeoutInSeconds();
		
		// if more customizations are needed, create your own Setup manually, e.g. see RedisInsertMessageHandlerTests
		fixture.UnmockAndWait<ISampleMongoRepository, SampleMongoModel, CancellationToken>(
			fixture.SampleMongoRepoMock,
			src => src.InsertSampleModel(It.IsAny<SampleMongoModel>(), It.IsAny<CancellationToken>()),
			cts, TimeSpan.FromSeconds(2));
		
		// Act
		await fixture.PublishToRabbitAndWaitForConsumptionAsync(new InsertMongoMessage("My sample text"), "#.CustomRoutingKey.#", cts);
		
		// Assert
		var filter = new FilterDefinitionBuilder<SampleMongoModel>().Eq("Text", "My sample text");
		var result = await fixture.MongoClient.GetDatabase("TestingSamplesDatabase")
					.GetCollection<SampleMongoModel>("SampleCollection").Find(filter).ToListAsync();

		result.Should().ContainSingle();
	}
}