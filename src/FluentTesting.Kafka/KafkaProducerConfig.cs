using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace Testing.Kafka
{
	/// <summary>
	/// Kafka producer config
	/// </summary>
	public class KafkaProducerConfig : ProducerConfig
	{
		public required SchemaRegistryConfig SchemaRegistryConfig { get; set; }

		public required AvroSerializerConfig AvroSerializerConfig { get; set; }
	}
}
