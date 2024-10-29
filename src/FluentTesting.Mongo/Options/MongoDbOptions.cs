using Testing.Common.Abstraction;

namespace Testing.Mongo.Options;

public class MongoDbOptions : IContainerOptions
{
	/// <summary>
	/// Client username.
	/// </summary>
	public string Username { get; set; } = "guest";

	/// <summary>
	/// Client password.
	/// </summary>
	public string Password { get; set; } = "guest";

	/// <summary>
	/// Main Database. Also used for authorization.
	/// </summary>
	public string DatabaseName { get; set; } = "TestingSamplesDatabase";


	/// <inheritdoc/>
	public int? Port { get; set; } = 27017;

	/// <inheritdoc/>
	public bool RunAdminTool { get; set; } = true;
}