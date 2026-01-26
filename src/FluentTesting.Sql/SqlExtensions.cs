using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using FluentTesting.Sql.Options;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace FluentTesting.Sql
{
    public static class SqlExtensions
    {
        private const int MsSqlPort = 1433;
        internal static SqlOptions SqlOptions = new();

        /// <summary>
        /// Use sql server in docker with initial seed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="seed">sql seed</param>
        /// <param name="configuration">configuration to set connection string</param>
        /// <param name="customOptions">custom options</param>
        /// <returns></returns>
        public static IApplicationFactoryBuilder UseSql(
            this IApplicationFactoryBuilder builder, string seed,
            Action<ConfigurationBuilder, SqlContainerSettings> configuration, Action<SqlOptions>? customOptions = null)
        {
            customOptions ??= _ => { };

            customOptions.Invoke(SqlOptions);

            var (SqlContainer, SqlClientContainer, SqlNetwork) = CreateSql(seed, builder.UseProxiedImages);

            builder.Containers.TryAdd(SqlOptions.ContainerName, SqlContainer);

            if (SqlClientContainer is not null)
            {
                builder.Containers.TryAdd(nameof(SqlClientContainer), SqlClientContainer);
            }

            builder.Networks.TryAdd(nameof(SqlNetwork), SqlNetwork);

            builder.Builders.Add(cknfBuilder => configuration.Invoke(cknfBuilder, new(GetConnectionString(SqlContainer))));

            return builder;
        }

        /// <summary>
        /// Use sql server in docker
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">configuration to set connection string</param>
        /// <param name="customOptions">custom options</param>
        /// <returns></returns>
        public static IApplicationFactoryBuilder UseSql(
            this IApplicationFactoryBuilder builder,
            Action<ConfigurationBuilder, SqlContainerSettings> configuration, Action<SqlOptions>? customOptions = null)
        {
            customOptions ??= _ => { };

            customOptions.Invoke(SqlOptions);

            var (SqlContainer, SqlClientContainer, SqlNetwork) = CreateSql(string.Empty, builder.UseProxiedImages);

            builder.Containers.TryAdd(SqlOptions.ContainerName, SqlContainer);

            if (SqlClientContainer is not null)
            {
                builder.Containers.TryAdd(nameof(SqlClientContainer), SqlClientContainer);
            }

            builder.Networks.TryAdd(nameof(SqlNetwork), SqlNetwork);

            builder.Builders.Add(cknfBuilder => configuration.Invoke(cknfBuilder, new(GetConnectionString(SqlContainer))));

            return builder;
        }

        /// <summary>
        /// Executes the SQL script in the MsSql container.
        /// </summary>
        /// <param name="scriptContent">The content of the SQL script to execute.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task that completes when the SQL script has been executed.</returns>
        internal static async Task<ExecResult> ExecMsSqlScriptAsync(this IContainer container, string scriptContent, CancellationToken ct = default)
        {
            var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

            await container.CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath)
                .ConfigureAwait(false);

            return await container.ExecAsync(["/opt/mssql-tools18/bin/sqlcmd", "-C", "-b", "-r", "1", "-U", SqlOptions.DefaultUsername, "-P", SqlOptions.Password, "-i", scriptFilePath], ct)
                .ConfigureAwait(false);
        }

        private static (IContainer SqlContainer, IContainer? SqlClientContainer, INetwork SqlNetwork)
            CreateSql(string seed, bool useProxiedImages)
        {
            var network = NetworkProvider.GetBasicNetwork();

            IContainer? clientContainer = null;

            var sqlContainerBuilder = new ContainerBuilder("mssql/server:2019-CU32-ubuntu-20.04".GetProxiedImagePath(useProxiedImages, "mcr.microsoft.com"))
                .WithNetwork(network)
                .WithCleanUp(true)
                .WithNetworkAliases("mssql")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("MSSQL_SA_PASSWORD", SqlOptions.Password)
                .WithPortBinding(SqlOptions.Port ?? MsSqlPort, MsSqlPort)
                .WithName($"TestContainers-MsSql-{Guid.NewGuid()}")
                .SetWaitStrategy(MsSqlPort, SqlOptions.WaitStrategy);

            if (SqlOptions.RunInExpressMode)
            {
                sqlContainerBuilder.WithEnvironment("MSSQL_PID", "Express");
            }
            else
            {
                sqlContainerBuilder.WithEnvironment("MSSQL_PID", "Developer");
            }

            var sqlContainer = sqlContainerBuilder.Build();

            var result = sqlContainer.EnsureContainer(async container =>
            {
                await container.ExecAsync(["/bin/bash", "-c", $"mkdir -p {SqlOptions.BackupPath}"]);

                if (SqlOptions.Database != "master")
                {
                    var resultCode = -1L;
                    var maxRetries = SqlOptions.InitWaitStrategy.RetryCount;
                    ExecResult dbInitResult = new();

                    while (resultCode != 0 && maxRetries > 0)
                    {
                        dbInitResult = await container.ExecAsync([
                        "/opt/mssql-tools18/bin/sqlcmd", "-C", "-b", "-r", "1", "-U",
                        SqlOptions.DefaultUsername, "-P", SqlOptions.Password,
                        "-Q", "SELECT 1"]);

                        await Task.Delay(TimeSpan.FromSeconds(SqlOptions.InitWaitStrategy.IntervalSeconds is int interval ? interval : 1));

                        resultCode = dbInitResult.ExitCode ?? -1;
                        maxRetries--;
                    }

                    resultCode = -1L;
                    maxRetries = SqlOptions.InitWaitStrategy.RetryCount;

                    while (resultCode != 0 && maxRetries > 0)
                    {
                        dbInitResult = await container.ExecAsync([
                        "/opt/mssql-tools18/bin/sqlcmd", "-C", "-b", "-r", "1", "-U",
                        SqlOptions.DefaultUsername, "-P", SqlOptions.Password,
                        "-Q", $"CREATE DATABASE {SqlOptions.Database}"]);

                        await Task.Delay(TimeSpan.FromSeconds(SqlOptions.InitWaitStrategy.IntervalSeconds is int interval ? interval : 1));

                        resultCode = dbInitResult.ExitCode ?? -1;
                        maxRetries--;
                    }

                    if (maxRetries < 0 && resultCode != 0)
                    {
                        return dbInitResult;
                    }
                }

                if (string.IsNullOrEmpty(seed))
                {
                    return new("", "", 0);
                }

                return await container.ExecMsSqlScriptAsync($"USE {SqlOptions.Database}; {seed}");
            });


            if (result.ExitCode != 0)
            {
                throw new Exception($"Sql initialisation failed: {result.Stderr} {result.Stdout}");
            }

            if (System.Diagnostics.Debugger.IsAttached && SqlOptions.RunAdminTool)
            {
                clientContainer = new ContainerBuilder()
                    .WithNetwork(network)
                    .WithCleanUp(true)
                    .WithImage("adminer".GetProxiedImagePath(useProxiedImages))
                    .WithEnvironment("ADMINER_DEFAULT_SERVER", "mssql")
                    .WithEnvironment("ADMINER_DEFAULT_USER", SqlOptions.DefaultUsername)
                    .WithEnvironment("ADMINER_DEFAULT_PASSWORD", SqlOptions.Password)
                    .WithEnvironment("ADMINER_DEFAULT_DB", SqlOptions.Database)
                    .WithName($"TestContainers-SqlAdminer-{Guid.NewGuid()}")
                    .WithPortBinding(8080, 8080)
                    .Build();

                clientContainer.EnsureContainer();
            }

            return (sqlContainer, clientContainer, network);
        }

        /// <summary>
        /// Gets the MsSql connection string.
        /// </summary>
        /// <returns>The MsSql connection string.</returns>
        private static string GetConnectionString(IContainer container)
        {
            var properties = new Dictionary<string, string>
            {
                { "Server", container.Hostname + "," + container.GetMappedPublicPort(MsSqlPort) },
                { "Database", SqlOptions.Database },
                { "User Id", SqlOptions.DefaultUsername },
                { "Password", SqlOptions.Password },
                { "Encrypt", bool.TrueString },
                { "TrustServerCertificate", bool.TrueString }
            };
            return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
        }
    }
}
