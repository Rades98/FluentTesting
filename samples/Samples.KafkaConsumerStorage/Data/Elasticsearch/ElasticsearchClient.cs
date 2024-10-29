using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using Samples.KafkaConsumerStorage.Contracts;

namespace Samples.KafkaConsumerStorage.Data.Elasticsearch
{
	internal class ElasticsearchClient :
		ElasticClient
	{
		public ElasticsearchClient(IOptions<ElasticSearchSettings> elasticConf) : base(new ConnectionSettings(new StaticConnectionPool(elasticConf.Value.Nodes.Select(node => new Uri(node))))
				.PrettyJson()
				.DefaultMappingFor<InsertObject>(m => m
					.IndexName(nameof(InsertObject)
					.ToLowerCaseWithHyphen())))
		{
			Indices.Create(nameof(InsertObject).ToLowerCaseWithHyphen(), i => i.Map<InsertObject>(x => x.AutoMap()));
		}
	}

	file static class ElasticExtensions
	{
		internal static string ToLowerCaseWithHyphen(this string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			return string.Concat(input.Select((c, i) => i > 0 && char.IsUpper(c) ? "-" + char.ToLower(c) : c.ToString().ToLower()));
		}
	}
}
