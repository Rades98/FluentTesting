using FluentAssertions;
using FluentTesting.Asp.Extensions;
using FluentTesting.Sql.Extensions;
using Samples.AspApp.Tests.Shared;

namespace Samples.AspApp.Tests.SQL
{
   [Collection("AspTestFixture")]
   public class SqlTests(TestFixture fixture)
   {
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

         //Assert against DB
         var obj = await fixture.GetMsSqlObjectAsync<SomeTable, int>("SomeTable", 1, "Id");

         obj?.SomeString.Should().Be("kokos");

         //Restore data
         await fixture.RestoreMsSqlDatabasesAsync();

         // Data should be "untouched"
         var res2 = await fixture.Client.GetAsync("sql");

         res2.AssertStatusCode(System.Net.HttpStatusCode.OK);

         await fixture.AssertJsonResponseAsync(res2, "AssertSqlResponse.json");
      }

      [Fact]
      public async Task SqlExtensionse_SHouldWork()
      {
         var obj2 = await fixture.GetMsSqlObjectAsync<SomeTable, int>("SomeTable", 1, "Id");

         obj2?.SomeString.Should().Be("string");

         var collectionOfAll = await fixture.GetMsSqlCollectionAsync<SomeTable>("SomeTable");

         collectionOfAll.Should().HaveCount(4);

         try
         {
            var failingCollection = await fixture.GetMsSqlObjectAsync<List<SomeTable>, int>("SomeTable", 1, "Id");
         }
         catch (Exception ex)
         {
            ex.Message.Should().Be("TObject cannot be a collection type.");
         }
      }

      [Fact]
      public async Task SqlExtensions_Base_ShouldWork()
      {
         var obj2 = await fixture.GetMsSqlObjectWithBaseAsync<SomeTableWithBase, int>("SomeTable", "SomeTableBase", 1, "Id", "Id");

         var x = 1;
         x++;
      }
   }
}