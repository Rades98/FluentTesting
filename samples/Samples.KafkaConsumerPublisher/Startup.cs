using Alza.Client.Kafka.Consumer;
using Alza.Platform.Applications.BackgroundWorker;
using Samples.KafkaConsumerPublisher.Services;
using System.Reflection;

namespace Samples.KafkaConsumerPublisher
{
	internal class Startup(IConfiguration configuration) : IBackgroundWorkerStartup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<IAvroHandlingService, AvroHandlingService>();

			// Kafka
			services.ConfigureKafkaConsumer(configuration, Assembly.GetExecutingAssembly(),
						opts => opts
								.UseDefaultExceptionHandler());
		}
	}
}
