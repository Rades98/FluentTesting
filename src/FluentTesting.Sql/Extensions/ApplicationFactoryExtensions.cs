using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Interfaces;
using FluentTesting.Sql.Options;
using System.Text;
using System.Text.Json;

namespace FluentTesting.Sql.Extensions
{
	/// <summary>
	/// MsSQL related application factory extensions
	/// </summary>
	public static partial class ApplicationFactoryExtensions
	{
		/// <summary>
		/// Backup databases - ignores master, so if you want to use such extension, set database name in 
		/// <see cref="SqlOptions"/> defined in 
		/// <see cref="SqlExtensions.UseSql(IApplicationFactoryBuilder, string, Action{Microsoft.Extensions.Configuration.ConfigurationBuilder, SqlContainerSettings}, Action{SqlOptions}?)"/>
		/// </summary>
		public static async Task<ExecResult> BackupMsSqlDatabasesAsync(this IApplicationFactory factory)
		{
			var msSqlContainer = factory.GetSqlContainer();
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
			var msSqlContainer = factory.GetSqlContainer();
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
		public static async Task<TObject> GetMsSqlObjectAsync<TObject, TIdentifier>
			(this IApplicationFactory factory, string tableName, TIdentifier key, string? identifierName = null)
			where TObject : class, new()
			where TIdentifier : notnull
		{
			var command = new StringBuilder($"USE {SqlExtensions.SqlOptions.Database}; SELECT TOP(1) * FROM {tableName} where ");

			if (identifierName is not null)
			{
				command.Append($"{identifierName} = ");
			}
			else
			{
				command.Append($"{tableName}Id = ");
			}

			if (key is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal)
			{
				command.Append(key);
			}
			else
			{
				command.Append($"'{key}'");
			}

			var msSqlContainer = factory.GetSqlContainer();
			var result = await msSqlContainer.ExecMsSqlScriptAsync(command.ToString());

			return JsonSerializer.Deserialize<TObject>(result.Stdout.ParseSqlResponseToJson())!;
		}

		/// <summary>
		/// Get raw result from sql
		/// </summary>
		/// <typeparam name="TIdentifier">type of Identifier</typeparam>
		/// <param name="tableName">Name of table</param>
		/// <param name="key">key</param>
		/// <param name="identifierName">key name - if not provided, logic tableName+Id will be applied</param>
		/// <returns></returns>
		public static async Task<string> GetRawMsSqlObjectAsync<TIdentifier>
			(this IApplicationFactory factory, string tableName, TIdentifier key, string? identifierName = null)
			where TIdentifier : notnull
		{
			var command = new StringBuilder($"USE {SqlExtensions.SqlOptions.Database}; SELECT TOP(1) * FROM {tableName} where ");

			if (identifierName is not null)
			{
				command.Append($"{identifierName} = ");
			}
			else
			{
				command.Append($"{tableName}Id = ");
			}

			if (key is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal)
			{
				command.Append(key);
			}
			else
			{
				command.Append($"'{key}'");
			}

			var msSqlContainer = factory.GetSqlContainer();
			var result = await msSqlContainer.ExecMsSqlScriptAsync(command.ToString());

			return result.Stdout;
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

		private static IContainer GetSqlContainer(this IApplicationFactory factory)
			=> factory.Containers.First(x => x.Key == SqlOptions.ContainerName).Value;

		private static string ParseSqlResponseToJson(this string response)
		{
			var lines = response.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
			var headerLine = lines[0];
			var separatorLine = lines[1];
			var dataLine = lines[2];
			var columns = ParseColumns(headerLine, separatorLine);
			var result = new Dictionary<string, object>();

			foreach (var (ColumnName, StartIndex, Length) in columns)
			{
				var value = dataLine.Substring(StartIndex, Length).Trim();
				result[ColumnName] = ParseValue(value);
			}

			return JsonSerializer.Serialize(result);
		}

		private static List<(string ColumnName, int StartIndex, int Length)> ParseColumns(string headerLine, string separatorLine)
		{
			var columns = new List<(string ColumnName, int StartIndex, int Length)>();
			var startIndex = 0;

			while (startIndex < separatorLine.Length)
			{
				if (separatorLine[startIndex] == '-')
				{
					var endIndex = startIndex;
					while (endIndex < separatorLine.Length && separatorLine[endIndex] == '-') endIndex++;

					var columnName = headerLine[startIndex..endIndex].Trim();
					var length = endIndex - startIndex;
					columns.Add((columnName, startIndex, length));
					startIndex = endIndex;
				}
				else
				{
					startIndex++;
				}
			}

			return columns;
		}

		private static object ParseValue(string value)
		{
			if (int.TryParse(value, out var intValue)) return intValue;
			if (long.TryParse(value, out var longValue)) return longValue;
			if (double.TryParse(value, out var doubleValue)) return doubleValue;
			if (decimal.TryParse(value, out var decimalValue)) return decimalValue;
			if (float.TryParse(value, out var floatValue)) return floatValue;
			return value;
		}
	}
}
