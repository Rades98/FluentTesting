using StackExchange.Redis;

namespace Samples.AspApp.Adapters.Redis
{
	internal class RedisAdapter : IRedisAdapter
	{
		private readonly IDatabase _database;

		public RedisAdapter(IConfiguration configuration)
		{
			var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString")!);
			_database = connectionMultiplexer.GetDatabase();
		}

		public async Task SetValueAsync(string key, string value, TimeSpan? expiry = null)
		{
			await _database.StringSetAsync(key, value, expiry);
		}

		public async Task<string?> GetValueAsync(string key)
		{
			var value = await _database.StringGetAsync(key);
			return value.HasValue ? value.ToString() : null;
		}
	}
}
