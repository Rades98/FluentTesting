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
    }
}
