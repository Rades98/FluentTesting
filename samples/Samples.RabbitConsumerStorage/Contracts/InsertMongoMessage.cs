using Alza.Client.Rabbit.Contract;

namespace Samples.RabbitConsumerStorage.Contracts;

public class InsertMongoMessage(string text) : IMessage
{
	public string Text { get; set; } = text;
}