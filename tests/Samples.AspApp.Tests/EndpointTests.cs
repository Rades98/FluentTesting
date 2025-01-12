using FluentAssertions;
using FluentTesting.Asp.Extensions;
using FluentTesting.Azurite.Extensions;
using FluentTesting.Sql.Extensions;
using Samples.AspApp.Tests.Shared;

namespace Samples.AspApp.Tests
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

		[Fact]
		public async Task SqlEndpoint_Should_ReturnOkAndData()
		{
			var res = await fixture.Client.GetAsync("sql");

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await fixture.AssertJsonResponseAsync(res, "AssertSqlResponse.json");
		}

		[Fact]
		public async Task BlobEndpoint_ShouldReturnOK()
		{
			var res = await fixture.Client.GetAsync("file");

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await fixture.AssertFileResponseAgainstBlobMd5Async(res, "photos", "asd.png");
		}

		[Fact]
		public async Task SqlPutEndpoint_Should_ReturnOk()
		{
			//Create backup
			await fixture.BackupMsSqlDatabasesAsync();

			var res = await fixture.Client.PutAsync("sql", null);

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			//Assert against DB
			var obj = await fixture.GetMsSqlObjectAsync<SomeTable, int>("SomeTable", 1, "Id");

			obj?.SomeString.Should().Be("kokos");

			//Restore data
			await fixture.RestoreMsSqlDatabasesAsync();

			// Data should be "untouched"
			var res2 = await fixture.Client.GetAsync("sql");

			res2.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await fixture.AssertJsonResponseAsync(res2, "AssertSqlResponse.json");

			var obj2 = await fixture.GetMsSqlObjectAsync<SomeTable, int>("SomeTable", 1, "Id");

			obj2?.SomeString.Should().Be("string");
		}
	}
}