namespace Samples.RabbitConsumerStorage
{
	public class Worker : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
			}
		}
	}
}