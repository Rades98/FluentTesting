using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.RabbitMq.Options;

namespace FluentTesting.RabbitMq.Containers
{
    internal static class RabbitMqContainerUtils
    {
        internal const int RabbitMqPort = 5672;

        internal static IContainer GetRabbitMqContainer(
            INetwork network,
            RabbitMqOptions rabbitOpts,
            IEnumerable<Exchange> consumerBindings,
            IEnumerable<Exchange> publisherBindings,
            bool useProxiedImages)
        {
            IContainer rabbitMqContainer;

            if (System.Diagnostics.Debugger.IsAttached && rabbitOpts.RunAdminTool)
            {
                rabbitMqContainer = new ContainerBuilder()
                    .WithNetwork(network)
                    .WithCleanUp(true)
                    .WithImage("rabbitmq:3.11-management".GetProxiedImagePath(useProxiedImages))
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
            }
            else
            {
                rabbitMqContainer = new ContainerBuilder()
                    .WithNetwork(network)
                    .WithCleanUp(true)
                    .WithImage("rabbitmq:3.11".GetProxiedImagePath(useProxiedImages))
                    .WithName($"TestContainers-RabbitMq-{Guid.NewGuid()}")
                    .WithEnvironment("RABBITMQ_DEFAULT_USER", RabbitMqOptions.UserName)
                    .WithEnvironment("RABBITMQ_DEFAULT_PASS", RabbitMqOptions.Password)
                    .WithPortBinding(rabbitOpts.Port ?? RabbitMqPort, 5672)
                    .WithWaitStrategy(Wait
                        .ForUnixContainer()
                        .UntilMessageIsLogged("Server startup complete"))
                    .Build();
            }

            rabbitMqContainer.EnsureContainer(async container =>
            {
                var results = new List<ExecResult>();

                var userPrefix = $"rabbitmqadmin -u {RabbitMqOptions.UserName} -p {RabbitMqOptions.Password}";

                var execScript = $"{userPrefix} declare queue name={rabbitOpts.QueueName} durable=true";

                results.Add(await container.ExecAsync(["/bin/bash", "-c", execScript]));

                foreach (var binding in consumerBindings)
                {
                    results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare exchange name={binding.ExchangeName} type={binding.ExchangeType} durable=true"]));

                    foreach (var routingKey in binding.RoutingKeys)
                    {
                        results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare binding source={binding.ExchangeName} destination={rabbitOpts.QueueName} routing_key={routingKey}"]));
                    }
                }

                foreach (var binding in publisherBindings)
                {
                    results.Add(await container.ExecAsync(["/bin/bash", "-c", $"{userPrefix} declare exchange name={binding.ExchangeName} type={binding.ExchangeType} durable=true auto_delete=false"]));
                }

                if (results.Any(x => x.ExitCode != 0))
                {
                    return results.FirstOrDefault(x => x.ExitCode != 0);
                }

                return results.FirstOrDefault();
            });

            return rabbitMqContainer;
        }
    }
}
