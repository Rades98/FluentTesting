namespace Samples.RabbitConsumerStorage.Services;

public class SampleService : ISampleService
{
	readonly ILogger<SampleService> logger;

	public SampleService(ILogger<SampleService> logger)
	{
		this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}
	public Task<bool> IsEmpty(Guid i)
	{
		logger.LogInformation("Sample log: in isEmpty");
		return Task.FromResult(i == Guid.Empty);
	}
}