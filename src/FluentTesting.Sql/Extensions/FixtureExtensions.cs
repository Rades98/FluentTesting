using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Interfaces;

namespace FluentTesting.Sql.Extensions
{
	public static class FixtureExtensions
	{
		/// <summary>
		/// Backup databases - ignores master, so if you want to use such extension, set database name in 
		/// <see cref="SqlOptions"/> defined in 
		/// <see cref="SqlExtensions.UseSql(IApplicationFactoryBuilder, string, Action{Microsoft.Extensions.Configuration.ConfigurationBuilder, SqlContainerSettings}, Action{SqlOptions}?)"/>
		/// </summary>
		public static Task<ExecResult> BackupMsSqlDatabasesAsync(this ITestFixture fixture)
			=> fixture.ApplicationFactory.BackupMsSqlDatabasesAsync();

		/// <summary>
		/// Restore databases - ignores master, so if you want to use such extension, set database name in 
		/// <see cref="SqlOptions"/> defined in 
		/// <see cref="SqlExtensions.UseSql(IApplicationFactoryBuilder, string, Action{Microsoft.Extensions.Configuration.ConfigurationBuilder, SqlContainerSettings}, Action{SqlOptions}?)"/>
		/// </summary>
		public static Task<ExecResult> RestoreMsSqlDatabasesAsync(this ITestFixture fixture)
			=> fixture.ApplicationFactory.RestoreMsSqlDatabasesAsync();

		/// <summary>
		/// Get object from sql - note that there is no mapping mechanism, so result will be mapped to object via JSON deserialization, 
		/// in case that primitive mapping provided in this package is not working for your scenario, use <see cref="GetRawMsSqlObjectAsync"/>
		/// and handle mapping mechanism yourself
		/// </summary>
		/// <typeparam name="TObject">type of object representing data</typeparam>
		/// <typeparam name="TIdentifier">type of Identifier</typeparam>
		/// <param name="tableName">Name of table</param>
		/// <param name="key">key</param>
		/// <param name="identifierName">key name - if not provided, logic tableName+Id will be applied</param>
		/// <returns></returns>
		public static Task<TObject> GetMsSqlObjectAsync<TObject, TIdentifier>(this ITestFixture fixture, string tableName, TIdentifier key, string? identifierName = null)
			where TObject : class
			where TIdentifier : notnull
			=> fixture.ApplicationFactory.GetMsSqlObjectAsync<TObject, TIdentifier>(tableName, key, identifierName);

		/// <summary>
		/// Get raw result from sql
		/// </summary>
		/// <typeparam name="TIdentifier">type of Identifier</typeparam>
		/// <param name="tableName">Name of table</param>
		/// <param name="key">key</param>
		/// <param name="identifierName">key name - if not provided, logic tableName+Id will be applied</param>
		/// <returns></returns>
		public static Task<string> GetRawMsSqlObjectAsync<TIdentifier>(this ITestFixture fixture, string tableName, TIdentifier key, string? identifierName = null)
			where TIdentifier : notnull
			=> fixture.ApplicationFactory.GetRawMsSqlObjectAsync(tableName, key, identifierName);

		/// <summary>
		/// Get a collection of objects from SQL - note that there is no explicit mapping mechanism, so the result will be mapped to objects via JSON deserialization.
		/// </summary>
		/// <typeparam name="TObject">Type of object representing data</typeparam>
		/// <param name="tableName">Name of the table</param>
		/// <returns>A collection of deserialized objects of type <typeparamref name="TObject"/></returns>
		public static Task<List<TObject>> GetMsSqlCollectionAsync<TObject>(this ITestFixture fixture, string tableName)
			where TObject : class
			=> fixture.ApplicationFactory.GetMsSqlCollectionAsync<TObject>(tableName);
	}
}
