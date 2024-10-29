using Alza.BackendCore.Data.Redis;
using Alza.BackendCore.Redis;
using Alza.Library.Configurations.System;

using Microsoft.Extensions.Options;

namespace Samples.RabbitConsumerStorage.Data.Redis;

public class SampleRedisRepository : RedisRepository<SampleRedisRepository>, ISampleRedisRepository
{
	public SampleRedisRepository(IRedisService redis, IOptions<RedisStorageConfiguration> configuration)
		: base(redis, configuration)
	{
	}

	public Task SetSampleData(int id, string? text) => WithConnectionAsync(db => db.StringSetAsync($"SampleRedisKey_{id}", text));
}