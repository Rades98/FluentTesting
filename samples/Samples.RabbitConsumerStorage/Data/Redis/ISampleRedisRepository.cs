namespace Samples.RabbitConsumerStorage.Data.Redis;

public interface ISampleRedisRepository
{
	Task SetSampleData(int id, string? text);
}