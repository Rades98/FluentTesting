using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using FluentTesting.Kafka.Containers;
using FluentTesting.Kafka.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using Testing.Kafka.Serdes;

namespace FluentTesting.Kafka
{
    /// <summary>
    /// Kafka extensions
    /// </summary>
    public static class KafkaExtensions
    {
        private static readonly ConcurrentDictionary<string, IConsumer<string, ISpecificRecord>> consumers = new();

        private static KafkaOptions kafkaOpts = new();

        private static string BootstrapServerUri = "";
        private static string SchemaRegistryUri = "";

        private static CachedSchemaRegistryClient? schemaregistryClient = null;

        /// <summary>
        /// Use kafka container
        /// </summary>
        /// <param name="builder">factory builder</param>
        /// <param name="opts">Options <see cref="KafkaOptions"/></param>
        /// <param name="configuration">configuration for your needs, leave empty to use default which adds configuration for consumer and producer</param>
        /// <returns></returns>
        public static IApplicationFactoryBuilder UseKafka(
            this IApplicationFactoryBuilder builder, Action<KafkaOptions> opts, Action<ConfigurationBuilder, KafkaContainerSettings> configuration)
        {
            kafkaOpts = new KafkaOptions();

            opts.Invoke(kafkaOpts);

            var (KafkaContainer, SchemaRegistryContainer, KafkaNetwork, AkhqContainer) = CreateKafka(builder.UseProxiedImages);

            builder.Containers.TryAdd(nameof(KafkaContainer), KafkaContainer);
            builder.Containers.TryAdd(nameof(SchemaRegistryContainer), SchemaRegistryContainer);

            if (AkhqContainer is not null)
            {
                builder.Containers.TryAdd(nameof(AkhqContainer), AkhqContainer);
            }

            builder.Networks.TryAdd(nameof(KafkaNetwork), KafkaNetwork);

            BootstrapServerUri = KafkaContainer.GetBootstrapUri();
            SchemaRegistryUri = SchemaRegistryContainer.GetSchemaRegistryUri();

            builder.Builders.Add(confBuilder => configuration.Invoke(
                    confBuilder,
                    new(
                        KafkaContainer.GetBootstrapUri(),
                        SchemaRegistryContainer.GetSchemaRegistryUri(),
                        kafkaOpts)));

            builder.AppServices.Add((services, config) => services.Configure<KafkaProducerConfig>(string.Empty, conf =>
            {
                conf.BootstrapServers = KafkaContainer.GetBootstrapUri();
                conf.SchemaRegistryConfig = new()
                {
                    Url = SchemaRegistryContainer.GetSchemaRegistryUri(),
                    MaxCachedSchemas = int.MaxValue,
                };
                conf.AvroSerializerConfig = new()
                {
                    UseLatestVersion = false,
                    AutoRegisterSchemas = true,
                    SubjectNameStrategy = SubjectNameStrategy.Record,
                    BufferBytes = 100,
                    NormalizeSchemas = true
                };
            }));

            return builder;
        }

        /// <summary>
        /// Use kafka consumer
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IApplicationFactory UseKafkaAvroConsumer(this IApplicationFactory factory)
        {
            if (kafkaOpts.UseKafkaConsumption)
            {
                var schemaRegistryConfig = new SchemaRegistryConfig
                {
                    Url = SchemaRegistryUri,
                };

                schemaregistryClient ??= new CachedSchemaRegistryClient(schemaRegistryConfig);

                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = BootstrapServerUri,
                    GroupId = "avro-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                foreach (var topicName in kafkaOpts.TestConsumerTopicNames)
                {
                    var consumer = new ConsumerBuilder<string, ISpecificRecord>(consumerConfig)
                        .SetValueDeserializer(new MultiSchemaAvroDeserializer(schemaregistryClient).AsSyncOverAsync())
                        .Build();

                    consumer.Subscribe(topicName);

                    consumers.TryAdd(topicName, consumer);
                }
            }
            return factory;
        }

        /// <summary>
        /// Consume message from topic  consumers must be registered via <see cref="UseKafkaAvroConsumer"/>
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="_">fixture object implementing <see cref="IApplicationFactory"/></param>
        /// <param name="topic">Topic</param>
        /// <param name="timeToWait">Time to wait - default is 60 seconds</param>
        /// <returns></returns>
        public static async Task<ConsumeResult<string, T>?> ConsumeKafkaMessage<T>(this IApplicationFactory _, string topic, TimeSpan? timeToWait = null)
            where T : class, ISpecificRecord
        {
            var consumer = consumers[topic];

            var cts = new CancellationTokenSource(timeToWait ?? TimeSpan.FromSeconds(60));

            var consumeResult = await Task.Run(() => consumer.Consume(cts.Token), cts.Token).ConfigureAwait(false);

            return consumeResult is null ? null : new()
            {
                Message = new()
                {
                    Headers = consumeResult.Message.Headers,
                    Key = consumeResult.Message.Key,
                    Timestamp = consumeResult.Message.Timestamp,
                    Value = (T)consumeResult.Message.Value
                }
            };
        }

        private static (IContainer KafkaContainer, IContainer SchemaRegistryContainer, INetwork KafkaNetwork, IContainer? ClientContainer)
            CreateKafka(bool useProxiedImages)
        {
            var network = NetworkProvider.GetBasicNetwork();

            IContainer? clientContainer = null;

            var kafkaContainer = KafkaContainerUtils.GetKafkaContainer(network, kafkaOpts, useProxiedImages);
            var schemaRegistryContainer = KafkaContainerUtils.GetSchemaRegistryContainer(network, kafkaContainer, kafkaOpts, useProxiedImages);

            var script = string.Join(" && ", kafkaOpts.TopicNames
                .Concat(kafkaOpts.TestConsumerTopicNames)
                .Distinct()
                .Select(topicName =>
                    $"../../bin/kafka-topics --bootstrap-server 0.0.0.0:{KafkaContainerUtils.BrokerPort} --topic {topicName} --create --partitions 1 --replication-factor 1"));

            var execResult = kafkaContainer.EnsureContainer(cnt => cnt.ExecAsync(["/bin/bash", "-c", script]));

            if (execResult.ExitCode != 0)
            {
                throw new Exception("Kafka topics creation failed", new(execResult.Stderr));
            }

            schemaRegistryContainer.EnsureContainer();

            if (System.Diagnostics.Debugger.IsAttached && kafkaOpts.RunAdminTool)
            {
                clientContainer = KafkaContainerUtils.GetAkhqContainer(network, kafkaContainer, useProxiedImages);

                clientContainer.EnsureContainer();
            }

            return (kafkaContainer, schemaRegistryContainer, network, clientContainer);
        }
    }
}