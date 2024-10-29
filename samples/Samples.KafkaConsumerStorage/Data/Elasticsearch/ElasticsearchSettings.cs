using System.Collections.Generic;

namespace Samples.KafkaConsumerStorage.Data.Elasticsearch
{
	/// <summary>
	/// Elastic search settings
	/// </summary>
	internal class ElasticSearchSettings
	{
		/// <summary>
		/// Nodes
		/// </summary>
		public IEnumerable<string> Nodes { get; set; } = [];

		/// <summary>
		/// Default index
		/// </summary>
		public string DefaultIndex { get; set; } = string.Empty;
	}
}
