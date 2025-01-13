using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using FluentTesting.Redis.Options;
using Microsoft.Extensions.Configuration;

namespace FluentTesting.Redis;

public static class RedisExtensions
{
	const int RedisPort = 6379;

	static readonly RedisOptions redisOptions = new();

	/// <summary>
	/// Use Redis container, with configuration and (optional) custom options.
	/// </summary>
	public static IApplicationFactoryBuilder UseRedis(
		this IApplicationFactoryBuilder builder,
		Action<ConfigurationBuilder, RedisContainerSettings> configuration,
		Action<RedisOptions>? customOptions = null)
	{
		customOptions ??= _ => { };

		customOptions.Invoke(redisOptions);

		var (RedisContainer, RedisClientContainer, RedisNetwork) = CreateRedis(builder.UseProxiedImages);

		builder.Containers.TryAdd(nameof(RedisContainer), RedisContainer);
		builder.Networks.TryAdd(nameof(RedisNetwork), RedisNetwork);

		if (RedisClientContainer is not null)
		{
			builder.Containers.TryAdd(RedisOptions.ContainerName, RedisClientContainer);
		}

		builder.Builders.Add(
			configurationBuilder => configuration.Invoke(
				configurationBuilder,
				new RedisContainerSettings(RedisContainer.Hostname, RedisContainer.GetMappedPublicPort(RedisPort))
			)
		);

		return builder;
	}

	/// <summary>
	/// Creates and runs Redis container.
	/// </summary>
	static (IContainer RedisContainer, IContainer? RedisClientContainer, INetwork RedisNetwork) CreateRedis(bool useProxiedImages)
	{
		var network = NetworkProvider.GetBasicNetwork();

		var container = new ContainerBuilder()
			.WithImage("redis:7.0".GetProxiedImagePath(useProxiedImages))
			.WithCleanUp(true)
			.WithNetwork(network)
			.WithPortBinding(redisOptions.Port ?? RedisPort, RedisPort)
			.WithNetworkAliases("redis")
			.WithName($"TestContainers-Redis-{Guid.NewGuid()}")
			.WithWaitStrategy(Wait
				.ForUnixContainer()
				.UntilPortIsAvailable(RedisPort))
			.Build();

		container.EnsureContainer(async container =>
		{
			var results = new List<ExecResult>();

			foreach (var entry in redisOptions.Seed)
			{
				results.Add(await container.ExecAsync(["/bin/bash", "-c", $"redis-cli SET {entry.Key} '{entry.Value}'"]));
			}

			return results.Any(x => x.ExitCode != 0) ? results.First(x => x.ExitCode != 0) : results.First();
		});

		IContainer? clientContainer = null;

		if (System.Diagnostics.Debugger.IsAttached && redisOptions.RunAdminTool)
		{
			clientContainer = new ContainerBuilder()
			.WithImage("redis/redisinsight:2.56.0".GetProxiedImagePath(useProxiedImages))
			.WithCleanUp(true)
			.WithPortBinding(9071, 5540)
			.WithNetwork(network)
			.WithName($"TestContainers-RedisInsight-{Guid.NewGuid()}")
			.WithEnvironment("REDISINSIGHT_HOST", "redis")
			.WithEnvironment("REDISINSIGHT_REDIS_PORT", $"{RedisPort}")
			.WithVolumeMount("redisInsight", "/data")
			.Build();

			clientContainer.EnsureContainer();
		}

		return (container, clientContainer, network);
	}
}