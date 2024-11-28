using FluentTesting.Common.Abstraction;

namespace FluentTesting.Kafka.Options
{
    /// <summary>
    /// Kafka options
    /// </summary>
    public class KafkaOptions : IContainerOptions
    {
        public IEnumerable<string> TopicNames { get; set; } = [];

        /// <summary>
        /// Register kafka consumer config and start consumption of topics
        /// specified
        /// </summary>
        public bool UseKafkaConsumption { get; set; } = false;

        /// <inheritdoc/>
        public bool RunAdminTool { get; set; } = true;

        /// <inheritdoc/>
        public int? Port { get; set; }

        /// <summary>
        /// Schema registry port
        /// </summary>
        public int? SchemaRegistryPort { get; set; }

        /// <summary>
        /// Predefined topics to consume by test consumer
        /// If not filled, consumes all
        /// </summary>
        public IEnumerable<string> TestConsumerTopicNames { get; set; } = [];
    }
}
