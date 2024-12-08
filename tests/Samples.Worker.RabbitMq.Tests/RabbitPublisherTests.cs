using FluentAssertions;
using FluentTesting.Common.Extensions;
using FluentTesting.RabbitMq.Extensions;
using Samples.Worker.RabbitMq.Tests.Shared;

namespace Samples.Worker.RabbitMq.Tests
{
    [Collection("RabbitMqFixture")]
    public class RabbitPublisherTests(TestFixture fixture)
    {
        [Fact]
        public async Task PublishFromApp_ShouldWork()
        {
            var cts = fixture.GetCtsWithTimeoutInSeconds();

            await Task.Delay(1000);

            var consumed = await fixture.ApplicationFactory.ConsumeRabbitMqMessage("testQueue", cts.Token);

            consumed.Should().NotBeNull();

            consumed?.Payload.Should().Be("Hello, world!");
        }
    }
}