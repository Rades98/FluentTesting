using FluentTesting.Asp.Extensions;
using Samples.AspApp.Tests.Shared;

namespace Samples.AspApp.Tests.Endpoints
{
	[Collection("AspTestFixture")]
	public class EndpointTests(TestFixture fixture)
	{
		[Fact]
		public async Task AuthorizedEndpoint_Should_ReturnOk()
		{
			var res = await fixture.Client.GetAsUserAsync("auth", 1);

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await fixture.AssertJsonResponseAsync(res, "AssertAuthJson.json");
		}

		[Fact]
		public async Task AnonymousEndpoint_Should_ReturnOk()
		{
			var res = await fixture.Client.GetAsync("anonym");

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await fixture.AssertJsonResponseAsync(res, "AssertJson.json");
		}
	}
}
