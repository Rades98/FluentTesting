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
        /// Kill sql connections
        /// </summary>
        /// <param name="fixture"></param>
        /// <returns></returns>
        public static Task<ExecResult> KillConnectionsAsync(this ITestFixture fixture)
            => fixture.ApplicationFactory.KillConnectionsAsync();

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
        /// Get object from sql with base entity (INNER JOIN is used)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TIdentifier"></typeparam>
        /// <param name="fixture"></param>
        /// <param name="tableName">table name</param>
        /// <param name="baseTable">base table to join</param>
        /// <param name="key">key</param>
        /// <param name="identifierName">identifier in first table</param>
        /// <param name="baseIdentifierName">identifier in base table</param>
        /// <returns></returns>
        public static Task<TObject> GetMsSqlObjectWithBaseAsync<TObject, TIdentifier>(this ITestFixture fixture, string tableName, string baseTable, TIdentifier key, string identifierName, string baseIdentifierName)
          where TObject : class
          where TIdentifier : notnull
          => fixture.ApplicationFactory.GetMsSqlObjectWithBaseAsync<TObject, TIdentifier>(tableName, baseTable, key, identifierName, baseIdentifierName);

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

        /// <summary>
        /// Get a collection of objects from SQL with join
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fixture"></param>
        /// <param name="tableName">table name</param>
        /// <param name="baseTableName">base table name</param>
        /// <param name="tableKey">table key for join</param>
        /// <param name="baseKey">base table key for join</param>
        /// <returns></returns>
        public static Task<List<TObject>> GetMsSqlCollectionWithBaseAsync<TObject>(
          this ITestFixture fixture,
          string tableName,
          string baseTableName,
          string tableKey,
          string baseKey)
          where TObject : class
           => fixture.ApplicationFactory.GetMsSqlCollectionWithBaseAsync<TObject>(tableName, baseTableName, tableKey, baseKey);

        /// <summary>
        /// Creates snapshot of database
        /// </summary>
        public static Task<ExecResult> SnapshotMsSqlDatabasesAsync(this ITestFixture fixture)
            => fixture.ApplicationFactory.SnapshotMsSqlDatabasesAsync();
        /// <summary>
        /// Restores database from latest known snapshot
        /// </summary>
        public static Task<ExecResult> RestoreMsSqlFromSnapshotAsync(this ITestFixture fixture)
            => fixture.ApplicationFactory.RestoreMsSqlFromSnapshotAsync();
    }
}
