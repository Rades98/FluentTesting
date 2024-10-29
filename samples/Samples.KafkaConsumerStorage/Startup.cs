using Alza.Client.Kafka.Consumer;
using Alza.Platform.Applications.BackgroundWorker;
using Nest;
using Samples.KafkaConsumerStorage.Data;
using Samples.KafkaConsumerStorage.Data.Elasticsearch;
using Samples.KafkaConsumerStorage.Services;
using System.Reflection;

namespace Samples.KafkaConsumerStorage
{
	internal class Startup(IConfiguration configuration) : IBackgroundWorkerStartup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<ISomeService, SomeService>();
			services.AddTransient<ISampleElasticRepo, SampleElasticRepo>();

			//Elastic
			services.Configure<ElasticSearchSettings>(configuration.GetSection("ElasticsearchSettings"));
			services.AddTransient<IElasticClient, ElasticsearchClient>();

			// Kafka
			services.ConfigureKafkaConsumer(configuration, Assembly.GetExecutingAssembly(),
						opts => opts
								.UseDefaultExceptionHandler());
		}
	}
}
