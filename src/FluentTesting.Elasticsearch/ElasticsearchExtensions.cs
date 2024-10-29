using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Configuration;
using Testing.Common.Extensions;
using Testing.Common.Interfaces;
using Testing.Common.Providers;
using Testing.Elasticsearch.Containers;
using Testing.Elasticsearch.Options;

namespace Testing.Elasticsearch
{
	public static class ElasticsearchExtensions
	{
		private static ElasticsearchOptions elasticOpts = new();

		/// <summary>
		/// Use Elasticsearch container
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="configuration">Configuration delegate for your needs</param>
		/// <returns></returns>
		public static IApplicationFactoryBuilder UseElasticsearch
			(this IApplicationFactoryBuilder factory, Action<ConfigurationBuilder, ElasticsearchContainerSettings> configuration, Action<ElasticsearchOptions>? opts = null)
		{
			opts ??= _ => { };

			opts.Invoke(elasticOpts);

			var (ElasticContainer, ElasticClientContainer, ElasticNetwork) = CreateElastic(factory.UseProxiedImages);

			factory.Containers.TryAdd(nameof(ElasticContainer), ElasticContainer);

			if (ElasticClientContainer is not null)
			{
				factory.Containers.TryAdd(nameof(ElasticClientContainer), ElasticClientContainer);
			}

			factory.Networks.TryAdd(nameof(ElasticNetwork), ElasticNetwork);

			factory.Builders.Add(confBuilder => configuration.Invoke(confBuilder,
				new([$"http://{ElasticContainer.Hostname}:{ElasticContainer.GetMappedPublicPort(ElasticsearchContainerUtils.ElasticPort)}"])));

			return factory;
		}

		private static (IContainer ElasticContainer, IContainer? ElasticClientContainer, INetwork ElasticNetwork) CreateElastic(bool useProxiedImages)
		{
			var network = NetworkProvider.GetBasicNetwork();

			IContainer? clientContainer = null;

			var container = ElasticsearchContainerUtils.GetElasticContainer(network, elasticOpts, useProxiedImages);

			var script = string.Join(" ", elasticOpts.IndexPatterns.Select(pattern => $"curl -X PUT localhost:{elasticOpts.Port ?? ElasticsearchContainerUtils.ElasticPort}/{pattern}"));

			var execResult = container.EnsureContainer(cnt => cnt.ExecAsync(["/bin/bash", "-c", script]));

			if (execResult.ExitCode != 0)
			{
				throw new Exception("Kafka topics creation failed", new(execResult.Stderr));
			}

			if (System.Diagnostics.Debugger.IsAttached && elasticOpts.RunAdminTool)
			{
				clientContainer = ElasticsearchContainerUtils.GetKibanaContainer(network, useProxiedImages);

				var kibanaScripts = elasticOpts.IndexPatterns.Select(pattern =>
$"curl POST kibana:5601/api/index_patterns/index_pattern -H 'Content-Type: application/json' -H 'kbn-xsrf: true' -d'{{ \"index_pattern\": {{\"title\": \"{pattern}\",\"timeFieldName\": \"joined_at\" }} }}'");

				var kibanaExecResult = clientContainer.EnsureContainer(async cnt =>
				{
					var execResults = new List<ExecResult>();

					foreach (var script in kibanaScripts)
					{
						var res = await cnt.ExecAsync(["/bin/bash", "-c", script]).ConfigureAwait(false);

						execResults.Add(res);
					}

					return execResults.FirstOrDefault();

				}, TimeSpan.FromSeconds(4));

				if (kibanaExecResult.ExitCode != 0)
				{
					throw new Exception("Kafka topics creation failed", new(kibanaExecResult.Stderr));
				}
			}

			return (container, clientContainer, network);
		}
	}
}
