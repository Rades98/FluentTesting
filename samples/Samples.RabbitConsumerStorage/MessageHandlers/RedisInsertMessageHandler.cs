using Alza.Client.Rabbit.Contract;

using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Data.Redis;

namespace Samples.RabbitConsumerStorage.MessageHandlers;

public class RedisInsertMessageHandler(ISampleRedisRepository repository) : IMessageHandler<InsertRedisMessage>
{
	public async Task<bool> Handle(InsertRedisMessage request, CancellationToken cancellationToken)
	{
		await repository.SetSampleData(request.Id, request.Text);
		return true;
	}
}