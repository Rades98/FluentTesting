using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using FluentTesting.RabbitMq.Containers;
using FluentTesting.RabbitMq.Options;
using Microsoft.Extensions.Configuration;

namespace FluentTesting.RabbitMq
{
    public static class RabbitMqExtensions
    {
        private static readonly RabbitMqOptions rabbitOpts = new();
        private static RabbitMqContainerSettings? rabbitSettings;

        /// <summary>
		/// Use RabbitMQ container with defined bindings
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration">configuration for your needs, leave empty to use default which adds configuration for consumer and publisher</param>
		/// <param name="customOptions">Options to change username, pw, queue name <see cref="RabbitMqOptions"/></param>
		public static IApplicationFactoryBuilder UseRabbitMq(this IApplicationFactoryBuilder builder,
            Action<ConfigurationBuilder, RabbitMqContainerSettings> configuration,
            Action<RabbitMqOptions>? customOptions = null)
        {
            customOptions ??= _ => { };

            customOptions.Invoke(rabbitOpts);

            var (RabbitMqContainer, RabbitNetwork) = CreateRabbit(rabbitOpts.ConsumerBindings, rabbitOpts.PublisherBindings, builder.UseProxiedImages);

            builder.Containers.TryAdd(RabbitMqOptions.ContainerName, RabbitMqContainer);
            builder.Networks.TryAdd(nameof(RabbitNetwork), RabbitNetwork);


            rabbitSettings = new RabbitMqContainerSettings(RabbitMqOptions.UserName, RabbitMqOptions.Password,
                        rabbitOpts.DefaultQueueName, "localhost", RabbitMqContainer.Container.GetMappedPublicPort(RabbitMqContainerUtils.RabbitMqPort),
                        [.. rabbitOpts.ConsumerBindings], [.. rabbitOpts.PublisherBindings], $"{rabbitOpts.DefaultQueueName}_dlx");

            builder.Builders.Add(confBuilder => configuration.Invoke(confBuilder, rabbitSettings));

            return builder;
        }

        private static (ContainerActionPair RabbitMqContainer, INetwork RabbitNetwork) CreateRabbit(IEnumerable<Exchange> consumerBindings,
            IEnumerable<Exchange> publisherBindings, bool useProxiedImages)
        {
            var network = NetworkProvider.GetBasicNetwork();

            var rabbitMqContainer = RabbitMqContainerUtils.GetRabbitMqContainer(network, rabbitOpts, consumerBindings, publisherBindings, useProxiedImages);

            return (rabbitMqContainer, network);
        }
    }
}
