using FluentTesting.Common.Abstraction;

namespace FluentTesting.RabbitMq.Options
{
    public class RabbitMqOptions : IContainerOptions
    {
        /// <summary>
        /// UserName
        /// </summary>
        public const string UserName = "username";

        /// <summary>
        /// Password
        /// </summary>
        public const string Password = "password";

        /// <summary>
        /// Password
        /// </summary>
        public const string ContainerName = "RabbitMqContainer";

        /// <summary>
        /// 
        /// </summary>
        internal const string AppConsumerQueueName = "FluentTestingAssertQueue";

        /// <summary>
        /// QueueName
        /// </summary>
        public string DefaultQueueName { get; set; } = "test";

        /// <summary>
        /// Consumer settings section name in configuration
        /// </summary>
        public string RabbitConsumerConfigurationName { get; set; } = "RabbitConsumerConfiguration";

        /// <summary>
        /// Publisher section name in configuration
        /// </summary>
        public string RabbitPublisherConfigurationName { get; set; } = "RabbitPublisherConfiguration";

        /// <summary>
        /// Consumer bindings
        /// </summary>
        public Exchange[] ConsumerBindings { get; set; } = [];

        /// <summary>
        /// Publisher bindings
        /// </summary>
        public Exchange[] PublisherBindings { get; set; } = [];

        /// <inheritdoc/>
        public int? Port { get; set; } = 5672;

        /// <inheritdoc/>
        public bool RunAdminTool { get; set; } = true;
    }
}
