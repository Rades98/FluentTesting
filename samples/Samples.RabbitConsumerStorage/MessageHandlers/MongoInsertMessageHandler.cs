using Alza.Client.Rabbit.Contract;

using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Data.Mongo;

namespace Samples.RabbitConsumerStorage.MessageHandlers;

public class MongoInsertMessageHandler(ISampleMongoRepository repository) : IMessageHandler<InsertMongoMessage>
{
	public async Task<bool> Handle(InsertMongoMessage message, CancellationToken cancellationToken)
	{
		await repository.InsertSampleModel(new SampleMongoModel{ Text = message.Text, Date = DateTime.UtcNow}, cancellationToken);
		return true;
	}
	
}