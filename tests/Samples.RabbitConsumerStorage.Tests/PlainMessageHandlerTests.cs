using FluentAssertions;

using Moq;

using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Services;
using Samples.RabbitConsumerStorage.Tests.Shared;

using Testing.Common.Extensions;
using Testing.RabbitMq;

using Xunit;

namespace Samples.RabbitConsumerStorage.Tests;

[Collection("RabbitConsumerWithMongoAndRedis")]
public class PlainMessageHandlerTests(TestFixture fixture)
{
	[Fact]
	public async Task HandlerReturns_True()
	{
		var cts = fixture.GetCtsWithTimeoutInSeconds();
	
		Task<bool>? result = null;
		
		// wrap the call in mock but call original implementation
		fixture.SampleServiceMock.Setup(s => s.IsEmpty(It.IsAny<Guid>())).Returns(
			(Guid guid) =>
				{
					result = fixture.UseBaseImplementationAndCancelToken<ISampleService, bool>(
						src => src.IsEmpty(guid),
						cts,
						TimeSpan.FromSeconds(1));
					return result;
				});
		
		await fixture.PublishToRabbitAndWaitForConsumptionAsync(new SampleGuidMessage(Guid.Empty), "#.CustomRoutingKey.#", cts);

		(await result!).Should().BeTrue();
	}
}