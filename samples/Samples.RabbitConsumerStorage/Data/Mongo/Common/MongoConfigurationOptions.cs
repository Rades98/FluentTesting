namespace Samples.RabbitConsumerStorage.Data.Mongo.Common;

public class MongoConfigurationOptions
{
	public string Username { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	public IEnumerable<string> Hosts { get; set; } = new List<string>();

	public int Port { get; set; } = -1;

	public string DatabaseName { get; set; } = string.Empty;

	public bool UseTls { get; set; }
}