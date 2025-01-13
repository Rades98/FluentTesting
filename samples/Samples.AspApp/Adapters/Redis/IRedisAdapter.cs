namespace Samples.AspApp.Adapters.Redis
{
	public interface IRedisAdapter
	{
		Task SetValueAsync(string key, string value, TimeSpan? expiry = null);

		Task<string?> GetValueAsync(string key);
	}
}
