using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.Elasticsearch.Options;

namespace FluentTesting.Elasticsearch.Containers
{
    internal static class ElasticsearchContainerUtils
    {
        internal const ushort ElasticPort = 9200;

        internal static IContainer GetElasticContainer(INetwork network, ElasticsearchOptions elasticOpts, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("library/elasticsearch:7.16.3".GetProxiedImagePath(useProxiedImages))
                .WithName($"TestContainers-Elasticsearch-{Guid.NewGuid()}")
                .WithNetwork(network)
                .WithNetworkAliases("elasticsearch")
                .WithEnvironment("discovery.type", "single-node")
                .WithEnvironment("xpack.security.enabled", "false")
                .WithEnvironment("ingest.geoip.downloader.enabled", "false")
                .WithPortBinding(System.Diagnostics.Debugger.IsAttached ? $"{elasticOpts.Port ?? ElasticPort}" : "", $"{ElasticPort}")
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(ElasticPort))
                .Build();

        internal static IContainer GetKibanaContainer(INetwork network, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("library/kibana:7.16.3".GetProxiedImagePath(useProxiedImages))
                .WithName($"TestContainers-Kibana-{Guid.NewGuid()}")
                .WithEnvironment("ELASTICSEARCH_HOSTS", $"http://elasticsearch:{ElasticPort}")
                .WithPortBinding(9889, 5601)
                .WithNetworkAliases("kibana")
                .WithNetwork(network)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5601))
                .Build();
    }
}
