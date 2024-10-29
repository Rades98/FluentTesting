using Alza.Client.Rabbit.Contract;

namespace Samples.RabbitConsumerStorage.Contracts;

public class SampleGuidMessage : IMessage
{
	public SampleGuidMessage(Guid guid)
	{
		UID = guid;
	}
	public Guid UID { get; set; }
}