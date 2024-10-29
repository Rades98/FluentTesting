using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Configuration;
using System.Text;
using Testing.Common.Extensions;
using Testing.Common.Interfaces;
using Testing.Common.Providers;
using Testing.Sql.Options;

namespace Testing.Sql
{
	public static class SqlExtensions
	{
		private const int MsSqlPort = 1433;
		private static SqlOptions sqlOptions = new();

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

			customOptions.Invoke(sqlOptions);

			var (SqlContainer, SqlClientContainer, SqlNetwork) = CreateSql(seed, builder.UseProxiedImages);

			builder.Containers.TryAdd(nameof(SqlContainer), SqlContainer);

			if (SqlClientContainer is not null)
			{
				builder.Containers.TryAdd(nameof(SqlClientContainer), SqlClientContainer);
			}

			builder.Networks.TryAdd(nameof(SqlNetwork), SqlNetwork);

			builder.Builders.Add(cknfBuilder => configuration.Invoke(cknfBuilder, new(GetConnectionString(SqlContainer))));

			return builder;
		}

		private static (IContainer SqlContainer, IContainer? SqlClientContainer, INetwork SqlNetwork)
			CreateSql(string seed, bool useProxiedImages)
		{
			var network = NetworkProvider.GetBasicNetwork();

			IContainer? clientContainer = null;

			var sqlContainer = new ContainerBuilder()
				.WithNetwork(network)
				.WithCleanUp(true)
				.WithNetworkAliases("mssql")
				.WithImage("mssql/server:2019-CU18-ubuntu-20.04".GetProxiedImagePath(useProxiedImages, "mcr.microsoft.com"))
				.WithEnvironment("ACCEPT_EULA", "Y")
				.WithEnvironment("SA_PASSWORD", sqlOptions.Password)
				.WithPortBinding(sqlOptions.Port ?? MsSqlPort, MsSqlPort)
				.WithName($"TestContainers-MsSql-{Guid.NewGuid()}")
				.WithWaitStrategy(Wait
					.ForUnixContainer()
					.UntilPortIsAvailable(sqlOptions.Port ?? MsSqlPort))
				.Build();

			var result = sqlContainer.EnsureContainer(_ => ExecScriptAsync(sqlContainer, seed));

			if (!string.IsNullOrEmpty(seed) && result.ExitCode != 0)
			{
				throw new Exception("Sql seed failed: " + result.Stderr);
			}

			if (System.Diagnostics.Debugger.IsAttached && sqlOptions.RunAdminTool)
			{
				clientContainer = new ContainerBuilder()
					.WithNetwork(network)
					.WithCleanUp(true)
					.WithImage("adminer".GetProxiedImagePath(useProxiedImages))
					.WithEnvironment("ADMINER_DEFAULT_SERVER", "mssql")
					.WithEnvironment("ADMINER_DEFAULT_USER", sqlOptions.DefautUsername)
					.WithEnvironment("ADMINER_DEFAULT_PASSWORD", sqlOptions.Password)
					.WithEnvironment("ADMINER_DEFAULT_DB", sqlOptions.Database)
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
				{ "Database", sqlOptions.Database },
				{ "User Id", sqlOptions.DefautUsername },
				{ "Password", sqlOptions.Password },
				{ "TrustServerCertificate", bool.TrueString }
			};
			return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
		}

		/// <summary>
		/// Executes the SQL script in the MsSql container.
		/// </summary>
		/// <param name="scriptContent">The content of the SQL script to execute.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Task that completes when the SQL script has been executed.</returns>
		private static async Task<ExecResult> ExecScriptAsync(IContainer container, string scriptContent, CancellationToken ct = default)
		{
			var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

			await container.CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
				.ConfigureAwait(false);

			return await container.ExecAsync(["/opt/mssql-tools/bin/sqlcmd", "-b", "-r", "1", "-U", sqlOptions.DefautUsername, "-P", sqlOptions.Password, "-i", scriptFilePath], ct)
				.ConfigureAwait(false);
		}
	}
}
