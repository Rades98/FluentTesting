using FluentAssertions;

using Moq;

using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Data.Redis;
using Samples.RabbitConsumerStorage.Tests.Shared;

using Testing.Common.Extensions;
using Testing.RabbitMq;

using Xunit;

namespace Samples.RabbitConsumerStorage.Tests;

[Collection("RabbitConsumerWithMongoAndRedis")]
public class RedisInsertMessageHandlerTests(TestFixture fixture)
{
	[Fact]
	public async Task HandlerReturns_True()
	{
		// Arrange
		const int Id = 123;
		const string Text = "Sample text for Redis";
		var cts = fixture.GetCtsWithTimeoutInSeconds();
	
		// wrap the call in mock but call original implementation
		// this is showing more customizable way, for simple calls use UnmockAndWait
		fixture.SampleRedisRepoMock.Setup(s => s.SetSampleData(It.IsAny<int>(), It.IsAny<string>())).Returns(
			(int id, string text) =>
				{
					return fixture.UseBaseImplementationAndCancelToken<ISampleRedisRepository>(
						src => src.SetSampleData(id, text),
						cts,
						TimeSpan.FromSeconds(2));
				});
				
		// Act
		
		await fixture.PublishToRabbitAndWaitForConsumptionAsync(new InsertRedisMessage {Id = Id, Text = Text}, "#.CustomRoutingKey.#", cts);
		
		// Assert
		var resultFromStorage = fixture.Redis.Connection.GetDatabase().StringGet($"SampleRedisKey_{Id}");
		resultFromStorage.ToString().Should().Be(Text);
	}
}