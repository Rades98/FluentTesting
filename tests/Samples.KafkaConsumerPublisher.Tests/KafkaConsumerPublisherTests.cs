using Alza.KafkaTest;
using FluentAssertions;
using Moq;
using Samples.KafkaConsumerPublisher.Contracts;
using Samples.KafkaConsumerPublisher.Services;
using Samples.KafkaConsumerPublisher.Tests.Shared;
using Testing.Common.Extensions;
using Testing.Kafka;

namespace Samples.KafkaConsumerPublisher.Tests
{
	[Collection("KafkaConsumerPublisher")]
	public class KafkaConsumerPublisherTests(TestsFixture fixture)
	{
		[Fact]
		public async Task HandlerHandlesAndPublishEvent()
		{
			var request = new SomeAwesomeAvro()
			{
				SomeDate = DateTime.Now,
				SomeString = "hello",
			};

			var cts = fixture.GetCtsWithTimeoutInSeconds();

			fixture.UnmockAndWait<IAvroHandlingService, bool, IncomingKafkaMessage, CancellationToken>(fixture.ServiceMock,
				src => src.EnrichAndPublishChangeEvent(It.IsAny<IncomingKafkaMessage>(), It.IsAny<CancellationToken>()), cts);

			await fixture.PublishAvroToKafkaAndWaitForConsumptionAsync(request, "incoming.topic", "someKey", null, cts);

			var consumptionResult = await fixture.ConsumeKafkaMessage<SomeOtherAvro>("outgoing.topic");

			consumptionResult.Should().NotBeNull();

			if (consumptionResult is not null)
			{
				consumptionResult.Message.Value.SomeString.Should().Be(request.SomeString);
			}
		}
	}
}