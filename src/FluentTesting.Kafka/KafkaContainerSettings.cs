using Testing.Kafka.Options;

namespace Testing.Kafka
{
	/// <summary>
	/// Kafka container settings
	/// </summary>
	/// <param name="BootstrapServer">Bootstrap url</param>
	/// <param name="SchemaRegistryUrl">Schema registry url</param>
	/// <param name="KafkaOptions">Kafka options <see cref="Options.KafkaOptions"/></param>
	public record KafkaContainerSettings(string BootstrapServer, string SchemaRegistryUrl, KafkaOptions KafkaOptions);
}
