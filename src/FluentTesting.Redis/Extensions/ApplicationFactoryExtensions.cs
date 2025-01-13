using DotNet.Testcontainers.Containers;
using FluentAssertions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Redis.Options;

namespace FluentTesting.Redis.Extensions
{
	public static class ApplicationFactoryExtensions
	{
		/// <summary>
		/// Get redis entry value specified by key
		/// </summary>
		/// <param name="factory">factory</param>
		/// <param name="key">key</param>
		/// <returns>value representation in string format</returns>
		public static async Task<string> GetRedisEntryValueAsync(this IApplicationFactory factory, string key)
		{
			var container = factory.GetRedisContainer();
			var res = await container.ExecAsync(["/bin/bash", "-c", $"redis-cli GET {key}"]);

			return res.Stdout.TrimEnd('\n');
		}

		/// <summary>
		/// Get all keys specified by pattern, if not filled * is used
		/// </summary>
		/// <param name="factory">factory</param>
		/// <param name="pattern">regex pattern</param>
		/// <returns>list of keys</returns>
		public static async Task<string[]> GetRedisKeysAsync(this IApplicationFactory factory, string? pattern = null)
		{
			var container = factory.GetRedisContainer();
			var res = await container.ExecAsync(["/bin/bash", "-c", $"redis-cli KEYS {pattern ?? "*"}"]);

			var keys = res.Stdout.Split("\n");
			return keys.Where(key => !string.IsNullOrEmpty(key)).ToArray();
		}

		/// <summary>
		/// Assert redis entry value obtained by key against provided value
		/// </summary>
		/// <param name="factory">factory</param>
		/// <param name="key">entry key</param>
		/// <param name="value">assertion value</param>
		public static async Task AssertRedisValueAsync(this IApplicationFactory factory, string key, string value)
		{
			var redisValue = await factory.GetRedisEntryValueAsync(key);

			value.Should().Be(redisValue);
		}

		private static IContainer GetRedisContainer(this IApplicationFactory factory)
			=> factory.Containers.First(x => x.Key == RedisOptions.ContainerName).Value;
	}
}
