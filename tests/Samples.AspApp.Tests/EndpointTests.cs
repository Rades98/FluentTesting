using FluentTesting.Asp.Extensions;
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
        public async Task SqlPutEndpoint_Should_ReturnOk()
        {
            //Create backup
            await fixture.BackupMsSqlDatabasesAsync();

            var res = await fixture.Client.PutAsync("sql", null);

            res.AssertStatusCode(System.Net.HttpStatusCode.OK);

            //Restore data
            await fixture.RestoreMsSqlDatabasesAsync();

            var res2 = await fixture.Client.GetAsync("sql");

            res2.AssertStatusCode(System.Net.HttpStatusCode.OK);

            await fixture.AssertJsonResponseAsync(res2, "AssertSqlResponse.json");
        }
    }
}