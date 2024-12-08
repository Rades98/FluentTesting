using FluentTesting.Common;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.RabbitMq;
using FluentTesting.RabbitMq.Options;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Samples.Worker.RabbitMq.ConsumptionHandlingServices;

namespace Samples.Worker.RabbitMq.Tests.Shared;

/// <summary>
/// Example of test fixture with custom options
/// </summary>
public class TestFixture : ITestFixture
{
    public IApplicationFactory ApplicationFactory { get; }

    public readonly Mock<IConsumptionHandler> ConsumptionHandlerMock = new();

    public TestFixture()
    {
        ApplicationFactory = new ApplicationFactoryBuilder<Program>()
            .RegisterServices((services, configuration) =>
                {
                    services.AddSingleton(ConsumptionHandlerMock.Object);
                })
            .UseRabbitMq((configuration, rabbitSettings) =>
            {
                configuration.AddObject("RabbitConnectionOptions", new RabbitConnectionOptions()
                {
                    HostName = rabbitSettings.Host,
                    Password = rabbitSettings.Password,
                    UserName = rabbitSettings.UserName,
                });
            }, opts =>
            {
                opts.PublisherBindings = [new Exchange() {
                    ExchangeName = "test",
                    RoutingKeys = ["testRoutingKey"]
                }];

                opts.ConsumerBindings = [

                    new Exchange()
                    {
                        ExchangeName = "consumptionTest",
                        RoutingKeys = ["RabbitMessage"],
                        QueueName = "ConsumptionTestRabbitMessageQueue"
                    }
                ];

                opts.DefaultQueueName = "testQueue";
            })
            .Build();
    }
}