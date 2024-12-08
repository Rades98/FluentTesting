using FluentTesting.RabbitMq.Options;

namespace FluentTesting.RabbitMq
{
    /// <summary>
	/// Rabbit container settings
	/// </summary>
	/// <param name="UserName">User name</param>
	/// <param name="Password">Password</param>
	/// <param name="QueueName">Queue name</param>
	/// <param name="Host">Rabbit host</param>
	/// <param name="Port">Rabbit port</param>
	/// <param name="ConsumerBindings">Consumer bindings</param>
	/// <param name="PublisherBindings">Publisher bindings</param>
	/// <param name="DlxQueue">Publisher bindings</param>
	public record RabbitMqContainerSettings(string UserName, string Password, string QueueName, string Host, int Port,
        Exchange[] ConsumerBindings, Exchange[] PublisherBindings, string DlxQueue);
}
