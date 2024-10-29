namespace Samples.RabbitConsumerStorage.Services;

public interface ISampleService
{
	public Task<bool> IsEmpty(Guid i);
}