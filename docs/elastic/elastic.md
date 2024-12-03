
# Elasticsearch
To use elasticsearch use extension method on `IApplicationFactoryBuilder` named `UseElasticSearch` with delegate containing `ConfigurationBuilder` and `ElasticSearchContainerSettings`. This will allow you to run elasticsearch in docker and register it within your fixture as follows.
This extension lives in package: `FluentTesting.Elasticsearch`

```csharp
.UseElasticsearch((configuration, elasticSettings) =>
{
	//Register with your specifications, this is just example
	configuration.AddObject("ElasticsearchSettings", new
	{
		Nodes = elasticSettings.NodesUrls,
	});
})
```