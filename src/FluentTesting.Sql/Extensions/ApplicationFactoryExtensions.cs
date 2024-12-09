using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Interfaces;
using FluentTesting.Sql.Options;

namespace FluentTesting.Sql.Extensions
{
    /// <summary>
    /// MsSQL related application factory extensions
    /// </summary>
    public static class ApplicationFactoryExtensions
    {
        /// <summary>
        /// Backup databases - ignores master, so if you want to use such extension, set database name in 
        /// <see cref="SqlOptions"/> defined in 
        /// <see cref="SqlExtensions.UseSql(IApplicationFactoryBuilder, string, Action{Microsoft.Extensions.Configuration.ConfigurationBuilder, SqlContainerSettings}, Action{SqlOptions}?)"/>
        /// </summary>
        public static async Task<ExecResult> BackupMsSqlDatabasesAsync(this IApplicationFactory factory)
        {
            var msSqlContainer = factory.Containers.First(x => x.Key == SqlOptions.ContainerName).Value;
            var dbNames = await msSqlContainer.GetDatabaseNamesAsync();

            var execResults = new List<ExecResult>();

            foreach (var database in dbNames)
            {
                execResults.Add(await msSqlContainer.ExecMsSqlScriptAsync($"BACKUP DATABASE [{database}] TO DISK = '{SqlOptions.BackupPath}/{database}.bak';"));
            }

            if (execResults.Any(x => x.ExitCode != 0))
            {
                return execResults.FirstOrDefault(x => x.ExitCode != 0);
            }

            return execResults.FirstOrDefault();
        }

        /// <summary>
        /// Restore databases - ignores master, so if you want to use such extension, set database name in 
        /// <see cref="SqlOptions"/> defined in 
        /// <see cref="SqlExtensions.UseSql(IApplicationFactoryBuilder, string, Action{Microsoft.Extensions.Configuration.ConfigurationBuilder, SqlContainerSettings}, Action{SqlOptions}?)"/>
        /// </summary>
        public static async Task<ExecResult> RestoreMsSqlDatabasesAsync(this IApplicationFactory factory)
        {
            var msSqlContainer = factory.Containers.First(x => x.Key == SqlOptions.ContainerName).Value;
            var dbNames = await msSqlContainer.GetDatabaseNamesAsync();

            var execResults = new List<ExecResult>();

            foreach (var database in dbNames)
            {
                execResults.Add(await msSqlContainer.ExecMsSqlScriptAsync(@$"
                    ALTER DATABASE {database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    GO
                    RESTORE DATABASE [{database}] FROM DISK = '{SqlOptions.BackupPath}/{database}.bak' WITH REPLACE;
                    GO
                    ALTER DATABASE {database} SET MULTI_USER;
                    GO
                "));
            }

            if (execResults.Any(x => x.ExitCode != 0))
            {
                return execResults.FirstOrDefault(x => x.ExitCode != 0);
            }

            return execResults.FirstOrDefault();
        }

        private static async Task<string[]> GetDatabaseNamesAsync(this IContainer container)
        {
            var res = await container.ExecMsSqlScriptAsync("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');");

            return res.Stdout
                    .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                    .Where(name =>
                        !string.IsNullOrWhiteSpace(name) &&
                        !name.Contains("name       ") &&
                        !name.Contains("----") &&
                        !name.Contains('(') &&
                        !name.Contains(')'))
                    .Select(name => name.Replace(" ", string.Empty))
                    .ToArray();
        }
    }
}
