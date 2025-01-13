using FluentTesting.Common.Abstraction;

namespace FluentTesting.Redis.Options;

/// <summary>
/// Redis options
/// </summary>
public class RedisOptions : IContainerOptions
{
	internal const string ContainerName = "RedisContainer";

	/// <inheritdoc/>
	public int? Port { get; set; } = 6001;

	/// <inheritdoc/>
	public bool RunAdminTool { get; set; } = true;

	public Dictionary<string, string> Seed = [];
}