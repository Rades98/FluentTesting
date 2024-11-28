using FluentTesting.Common.Abstraction;

namespace FluentTesting.Redis.Options;

/// <summary>
/// Redis options
/// </summary>
public class RedisOptions : IContainerOptions
{
    /// <inheritdoc/>
    public int? Port { get; set; } = 6001;

    /// <inheritdoc/>
    public bool RunAdminTool { get; set; } = true;
}