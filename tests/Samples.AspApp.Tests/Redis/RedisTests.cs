using FluentAssertions;
using FluentTesting.Asp.Extensions;
using FluentTesting.Redis.Extensions;
using Samples.AspApp.Tests.Shared;

namespace Samples.AspApp.Tests.Redis
{
	[Collection("AspTestFixture")]
	public class RedisTests(TestFixture fixture)
	{
		[Fact]
		public async Task RedisGetEndpoint_ShouldReturnData()
		{
			var res = await fixture.Client.GetAsync("redis");

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await res.AssertJsonObjectResponseAsync("\"some value :)\"");
		}

		[Fact]
		public async Task RedisUpdateEntry_ShouldWork()
		{
			var res = await fixture.Client.PutAsync("redis", null);

			res.AssertStatusCode(System.Net.HttpStatusCode.Accepted);

			var allKeys = await fixture.GetRedisKeysAsync();

			var val = await fixture.GetRedisEntryValueAsync("someKey");

			allKeys.Should().NotBeEmpty();
			allKeys.Length.Should().Be(5);
			val.Should().Be("some new value");

			await fixture.AssertRedisValueAsync("someKey", "some new value");
		}
	}
}
