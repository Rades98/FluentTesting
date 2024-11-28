using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.Kafka.Options;
using System.Collections.ObjectModel;
using System.Text;

namespace FluentTesting.Kafka.Containers
{
    internal static class KafkaContainerUtils
    {
        private const ushort ZookeeperPort = 2181;
        private const string StartupScriptFilePath = "/testcontainers.sh";

        internal const ushort KafkaPort = 9092;
        internal const ushort SchemaRegistryPort = 8085;
        internal const ushort BrokerPort = 9093;

        internal static IContainer GetKafkaContainer(INetwork network, KafkaOptions kafkaOpts, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("confluentinc/cp-kafka:7.4.1".GetProxiedImagePath(useProxiedImages))
                .WithNetwork(network)
                .WithName($"TestContainers-Kafka-{Guid.NewGuid()}")
                .WithNetworkAliases("kafka")
                .WithPortBinding(System.Diagnostics.Debugger.IsAttached ? $"{kafkaOpts.Port ?? KafkaPort}" : "", $"{KafkaPort}")
                .WithPortBinding(BrokerPort, true)
                .WithPortBinding(ZookeeperPort, true)
                .WithEnvironment("KAFKA_LISTENERS", "PLAINTEXT://0.0.0.0:" + KafkaPort + ",BROKER://0.0.0.0:" + BrokerPort)
                .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "BROKER:PLAINTEXT,PLAINTEXT:PLAINTEXT")
                .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "BROKER")
                .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "localhost:" + ZookeeperPort)
                .WithEnvironment("KAFKA_BROKER_ID", "1")
                .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
                .WithEnvironment("KAFKA_OFFSETS_TOPIC_NUM_PARTITIONS", "1")
                .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR", "1")
                .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_MIN_ISR", "1")
                .WithEnvironment("KAFKA_LOG_FLUSH_INTERVAL_MESSAGES", long.MaxValue.ToString())
                .WithEnvironment("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0")
                .WithEntrypoint("/bin/sh", "-c")
                .WithCommand("while [ ! -f " + StartupScriptFilePath + " ]; do sleep 0.1; done; " + StartupScriptFilePath)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("\\[KafkaServer id=\\d+\\] started"))
                .WithStartupCallback((container, ct) =>
                {
                    const char lf = '\n';
                    var startupScript = new StringBuilder();
                    startupScript.Append("#!/bin/bash");
                    startupScript.Append(lf);
                    startupScript.Append("echo 'clientPort=" + ZookeeperPort + "' > zookeeper.properties");
                    startupScript.Append(lf);
                    startupScript.Append("echo 'dataDir=/var/lib/zookeeper/data' >> zookeeper.properties");
                    startupScript.Append(lf);
                    startupScript.Append("echo 'dataLogDir=/var/lib/zookeeper/log' >> zookeeper.properties");
                    startupScript.Append(lf);
                    startupScript.Append("zookeeper-server-start zookeeper.properties &");
                    startupScript.Append(lf);
                    startupScript.Append("export KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://" + container.Hostname + ":" + container.GetMappedPublicPort(KafkaPort) + ",BROKER://" + container.IpAddress + ":" + BrokerPort);
                    startupScript.Append(lf);
                    startupScript.Append("echo '' > /etc/confluent/docker/ensure");
                    startupScript.Append(lf);
                    startupScript.Append("/etc/confluent/docker/run");
                    return container.CopyAsync(Encoding.Default.GetBytes(startupScript.ToString()), StartupScriptFilePath, Unix.FileMode755, ct);
                })
                .Build();

        internal static IContainer GetSchemaRegistryContainer(INetwork network, IContainer kafkaContainer, KafkaOptions kafkaOpts, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("confluentinc/cp-schema-registry:7.4.1".GetProxiedImagePath(useProxiedImages))
                .WithNetwork(network)
                .WithName($"TestContainers-SchemaRegistry-{Guid.NewGuid()}")
                .WithCleanUp(true)
                .WithEnvironment(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
                    {
                            { "SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", "PLAINTEXT://kafka:9093" },
                            { "SCHEMA_REGISTRY_KAFKASTORE_SECURITY_PROTOCOL", "PLAINTEXT"},
                            { "SCHEMA_REGISTRY_LISTENERS", "http://0.0.0.0:8085" },
                            { "SCHEMA_REGISTRY_HOST_NAME", "schemaregistry" }
                    }))
                .WithPortBinding(System.Diagnostics.Debugger.IsAttached ? $"{kafkaOpts.SchemaRegistryPort ?? SchemaRegistryPort}" : "", $"{SchemaRegistryPort}")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server started, listening for requests"))
                .WithNetworkAliases("schemaregistry")
                .DependsOn(kafkaContainer)
                .Build();

        internal static IContainer GetAkhqContainer(INetwork network, IContainer kafkaContainer, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("tchiotludo/akhq:0.23.0".GetProxiedImagePath(useProxiedImages))
                .WithNetwork(network)
                .WithName($"TestContainers-AKHQ-{Guid.NewGuid()}")
                .WithEnvironment(new Dictionary<string, string>
                {
                    { "AKHQ_CONFIGURATION", $@"
micronaut:
  server:
    cors:
      enabled: true

akhq:
  connections:
    local:
      properties:
        bootstrap.servers: ""kafka:9093""
      schema-registry:
        url: ""http://schema-registry:8085"""
                    }
                })
                .WithPortBinding(9000, 8080)
                .WithNetworkAliases("akhq")
                .DependsOn(kafkaContainer)
                .Build();
    }
}
