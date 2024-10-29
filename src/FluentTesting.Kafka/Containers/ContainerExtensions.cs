using DotNet.Testcontainers.Containers;

namespace Testing.Kafka.Containers
{
	internal static class ContainerExtensions
	{
		internal static string GetBootstrapUri(this IContainer kafkaContainer)
			=> kafkaContainer.GetConnectionString(KafkaContainerUtils.KafkaPort);

		internal static string GetSchemaRegistryUri(this IContainer kafkaContainer)
			=> kafkaContainer.GetConnectionString(KafkaContainerUtils.SchemaRegistryPort);

		private static string GetConnectionString(this IContainer? container, ushort port)
			=> $"{container!.Hostname}:{container.GetMappedPublicPort(port)}";
	}
}
