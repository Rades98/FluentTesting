using FluentTesting.Common;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.RabbitMq;
using FluentTesting.RabbitMq.Options;

namespace Samples.Worker.RabbitMq.Tests.Shared;

/// <summary>
/// Example of test fixture with custom options
/// </summary>
public class TestFixture : ITestFixture
{
    public IApplicationFactory ApplicationFactory { get; }

    public TestFixture()
    {
        ApplicationFactory = new ApplicationFactoryBuilder<Program>()
            .RegisterServices((services, configuration) =>
                {

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

                // Since we want to consume aswell (published message from tested app),
                // we need to add publisher bindings which will be asserter in tests
                opts.ConsumerBindings = [new Exchange() {
                    ExchangeName = "test",
                    RoutingKeys = ["testRoutingKey"]
                }];

                opts.QueueName = "testQueue";
            })
            .Build();
    }
}