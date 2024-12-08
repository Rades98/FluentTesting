using FluentAssertions;
using FluentTesting.Common.Extensions;
using FluentTesting.RabbitMq.Extensions;
using Moq;
using Samples.Worker.RabbitMq.ConsumptionHandlingServices;
using Samples.Worker.RabbitMq.Contracts;
using Samples.Worker.RabbitMq.Tests.Shared;

namespace Samples.Worker.RabbitMq.Tests
{
    [Collection("RabbitMqFixture")]
    public class RabbitConsumerTests(TestFixture fixture)
    {
        [Fact]
        public async Task ObtainingMessages_Should_Work()
        {
            var contract = new RabbitMessage()
            {
                BirthDate = DateTime.Now,
                Email = "coconut@coco.com",
                SurName = "Testschenko",
                Name = "Testicss"
            };

            var cts = fixture.GetCtsWithTimeoutInSeconds(60);

            // pseudo unmocking process to create callback to stop waiting for consumption, cause message has been consumed
            fixture.UnmockAndWait<IConsumptionHandler, bool, RabbitMessage, CancellationToken>(
                fixture.ConsumptionHandlerMock,
                src => src.HandleMessageAsync(It.IsAny<RabbitMessage>(), It.IsAny<CancellationToken>()),
                cts);

            await fixture.ApplicationFactory.PublishJsonToRabbitAndWaitForConsumption("consumptionTest", "RabbitMessage", contract, cts);

            // This should be asserted against db or handler should create new notification etc
            // This static object is just here for test purposes where i don't want to include db or smth
            // but it is filled in handler where message is handled
            StaticRabbitHandlerState.RabbitMessage.BirthDate.Should().Be(contract.BirthDate);
            StaticRabbitHandlerState.RabbitMessage.Email.Should().Be(contract.Email);
            StaticRabbitHandlerState.RabbitMessage.SurName.Should().Be(contract.SurName);
            StaticRabbitHandlerState.RabbitMessage.Name.Should().Be(contract.Name);
        }
    }
}
