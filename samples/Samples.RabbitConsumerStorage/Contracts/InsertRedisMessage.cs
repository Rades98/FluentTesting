using Alza.Client.Rabbit.Contract;

namespace Samples.RabbitConsumerStorage.Contracts;

public class InsertRedisMessage : IMessage
{
	public int Id { get; set; }
	
	public string? Text { get; set; }
}