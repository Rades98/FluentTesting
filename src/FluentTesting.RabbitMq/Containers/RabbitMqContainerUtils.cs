using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Extensions;
using FluentTesting.RabbitMq.Options;

namespace FluentTesting.RabbitMq.Containers
{
    internal static class RabbitMqContainerUtils
    {
        internal const int RabbitMqPort = 5672;

        internal static ContainerActionPair GetRabbitMqContainer(
            INetwork network,
            RabbitMqOptions rabbitOpts,
            IEnumerable<Exchange> consumerBindings,
            IEnumerable<Exchange> publisherBindings,
            bool useProxiedImages)
        {
            var rabbitMqContainer = new ContainerBuilder("rabbitmq:4.0-management".GetProxiedImagePath(useProxiedImages))
                    .WithNetwork(network)
                    .WithCleanUp(true)
                    .WithName($"TestContainers-RabbitMq-{Guid.NewGuid()}")
                    .WithEnvironment("RABBITMQ_DEFAULT_USER", RabbitMqOptions.UserName)
                    .WithEnvironment("RABBITMQ_DEFAULT_PASS", RabbitMqOptions.Password)
                    .WithEnvironment("RABBITMQ_DEFAULT_VHOST", "/")
                    .WithPortBinding(rabbitOpts.Port ?? RabbitMqPort, 5672)
                    .WithExposedPort(15672)
                    .WithPortBinding(9008, 15672)
                    .WithWaitStrategy(Wait
                        .ForUnixContainer()
                        .UntilMessageIsLogged("Server startup complete"))
                    .Build();

            return new(rabbitMqContainer, async cnt =>
            {
                var res = await cnt.EnsureContainerAsync(async container =>
                {
                    var results = new List<ExecResult>();

                    var userPrefix = $"rabbitmqadmin -u {RabbitMqOptions.UserName} -p {RabbitMqOptions.Password}";

                    results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare queue name={rabbitOpts.DefaultQueueName} durable=true"]));
                    results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare queue name={RabbitMqOptions.AppConsumerQueueName} durable=true"]));

                    foreach (var binding in consumerBindings)
                    {
                        results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare exchange name={binding.ExchangeName} type={binding.ExchangeType} durable=true"]));

                        if (binding.QueueName is not null)
                        {
                            results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare queue name={binding.QueueName} durable=true"]));
                        }

                        foreach (var routingKey in binding.RoutingKeys)
                        {
                            results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare binding source={binding.ExchangeName} destination={binding.QueueName ?? rabbitOpts.DefaultQueueName} routing_key={routingKey}"]));
                        }
                    }

                    foreach (var binding in publisherBindings)
                    {
                        results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare exchange name={binding.ExchangeName} type={binding.ExchangeType} durable=true auto_delete=false"]));

                        foreach (var routingKey in binding.RoutingKeys)
                        {
                            results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare binding source={binding.ExchangeName} destination={RabbitMqOptions.AppConsumerQueueName} routing_key={routingKey}"]));
                        }
                    }

                    if (results.Any(x => x.ExitCode != 0))
                    {
                        return results.FirstOrDefault(x => x.ExitCode != 0);
                    }

                    return results.FirstOrDefault();
                });

                return res;
            });
        }
    }
}
