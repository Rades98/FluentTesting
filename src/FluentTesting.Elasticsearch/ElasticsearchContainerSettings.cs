namespace Testing.Elasticsearch
{
	/// <summary>
	/// Elasticearch container settings 
	/// </summary>
	/// <param name="NodesUrls">Elastic nodes</param>
	public record ElasticsearchContainerSettings(IEnumerable<string> NodesUrls);
}
