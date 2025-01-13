using FluentTesting.Common.Interfaces;

namespace FluentTesting.Redis.Extensions
{
	public static class FixtureExtensions
	{
		/// <summary>
		/// Get redis entry value specified by key
		/// </summary>
		/// <param name="fixture">fixture</param>
		/// <param name="key">key</param>
		/// <returns>value representation in string format</returns>
		public static Task<string> GetRedisEntryValueAsync(this ITestFixture fixture, string key)
			=> fixture.ApplicationFactory.GetRedisEntryValueAsync(key);

		/// <summary>
		/// Get all keys specified by pattern, if not filled * is used
		/// </summary>
		/// <param name="fixture">fixture</param>
		/// <param name="pattern">regex pattern</param>
		/// <returns>list of keys</returns>
		public static Task<string[]> GetRedisKeysAsync(this ITestFixture fixture, string? pattern = null)
			=> fixture.ApplicationFactory.GetRedisKeysAsync(pattern);

		/// <summary>
		/// Assert redis entry value obtained by key against provided value
		/// </summary>
		/// <param name="fixture">fixture</param>
		/// <param name="key">entry key</param>
		/// <param name="value">assertion value</param>
		public static Task AssertRedisValueAsync(this ITestFixture fixture, string key, string value)
			=> fixture.ApplicationFactory.AssertRedisValueAsync(key, value);
	}
}
