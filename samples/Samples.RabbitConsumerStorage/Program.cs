using Alza.Platform.Applications.BackgroundWorker;

namespace Samples.RabbitConsumerStorage;

public class Program
{
	protected Program()
	{
	}
	
	static Task Main(string[] args)
	{
		return BackgroundWorkerHost.CreateDefaultHostBuilder<Startup>(args).Build().RunAsync();
	}
}