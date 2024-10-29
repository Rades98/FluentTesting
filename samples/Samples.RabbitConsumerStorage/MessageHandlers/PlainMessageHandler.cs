using Alza.Client.Rabbit.Contract;

using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Services;

namespace Samples.RabbitConsumerStorage.MessageHandlers;

public sealed class PlainMessageHandler(ISampleService service) : IMessageHandler<SampleGuidMessage>
{
	public Task<bool> Handle(SampleGuidMessage message, CancellationToken cancellationToken)
	{
		return service.IsEmpty(message.UID);
	}
}
