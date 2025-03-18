using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Interfaces;
using FluentTesting.Sql.Options;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentTesting.Sql.Extensions
{
   /// <summary>
   /// MsSQL related application factory extensions
   /// </summary>
   public static partial class ApplicationFactoryExtensions
   {
      private static readonly JsonSerializerOptions _jsonOptions = new()
      {
         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };

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

      /// <summary>
      /// Get object from SQL - note that there is no explicit mapping mechanism, so the result will be mapped to the object via JSON deserialization. 
      /// If the provided primitive mapping mechanism does not suit your scenario, use <see cref="GetRawMsSqlObjectAsync"/>
      /// Do not use with collections, if you want to obtain collection, use <see cref="GetMsSqlCollectionAsync"/>
      /// and handle mapping yourself.
      /// </summary>
      /// <typeparam name="TObject">Type of object representing data</typeparam>
      /// <typeparam name="TIdentifier">Type of Identifier</typeparam>
      /// <param name="tableName">Name of the table</param>
      /// <param name="key">Key</param>
      /// <param name="identifierName">Key name - if not provided, the logic `tableName + Id` will be applied</param>
      /// <returns>Deserialized object of type <typeparamref name="TObject"/></returns>
      public static async Task<TObject> GetMsSqlObjectAsync<TObject, TIdentifier>(
        this IApplicationFactory factory,
        string tableName,
        TIdentifier key,
        string? identifierName = null)
        where TObject : class
        where TIdentifier : notnull
      {
         if (typeof(System.Collections.IEnumerable).IsAssignableFrom(typeof(TObject)) && typeof(TObject) != typeof(string))
         {
            throw new ArgumentException($"TObject cannot be a collection type.");
         }

         var msSqlContainer = factory.GetSqlContainer();

         var columnMetadata = await msSqlContainer.GetMetadataAsync(tableName);

         var command = new StringBuilder($"USE {SqlExtensions.SqlOptions.Database}; SELECT TOP(1) * FROM {tableName} WHERE ");

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

         var dataResult = await msSqlContainer.ExecMsSqlScriptAsync(command.ToString());
         var parsedJson = ParseSqlResponseToJson(dataResult.Stdout, columnMetadata);
         return JsonSerializer.Deserialize<TObject>(parsedJson)!;
      }

      public static async Task<TObject> GetMsSqlObjectWithBaseAsync<TObject, TIdentifier>(
        this IApplicationFactory factory,
        string tableName,
        string baseTable,
        TIdentifier key,
        string identifierName,
        string baseIdentifierName)
        where TObject : class
        where TIdentifier : notnull
      {
         if (typeof(System.Collections.IEnumerable).IsAssignableFrom(typeof(TObject)) && typeof(TObject) != typeof(string))
         {
            throw new ArgumentException($"TObject cannot be a collection type.");
         }

         var msSqlContainer = factory.GetSqlContainer();

         var columnMetadata = await msSqlContainer.GetMetadataAsync(tableName);
         var baseColumnMetadata = await msSqlContainer.GetMetadataAsync(baseTable);
         columnMetadata.AddRange(baseColumnMetadata);

         var command = new StringBuilder($"USE {SqlExtensions.SqlOptions.Database}; SELECT TOP(1) * FROM {tableName} AS FST ");
         command.Append($"INNER JOIN {baseTable} AS SND ON FST.{identifierName} = SND.{baseIdentifierName} ");
         command.Append($"WHERE FST.{identifierName} = ");

         if (key is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal)
         {
            command.Append(key);
         }
         else
         {
            command.Append($"'{key}'");
         }

         var dataResult = await msSqlContainer.ExecMsSqlScriptAsync(command.ToString());
         var parsedJson = ParseSqlResponseToJson(dataResult.Stdout, columnMetadata);
         return JsonSerializer.Deserialize<TObject>(parsedJson)!;
      }

      /// <summary>
      /// Get a collection of objects from SQL - note that there is no explicit mapping mechanism, so the result will be mapped to objects via JSON deserialization.
      /// </summary>
      /// <typeparam name="TObject">Type of object representing data</typeparam>
      /// <param name="tableName">Name of the table</param>
      /// <returns>A collection of deserialized objects of type <typeparamref name="TObject"/></returns>
      public static async Task<List<TObject>> GetMsSqlCollectionAsync<TObject>(
        this IApplicationFactory factory,
        string tableName)
        where TObject : class
      {
         var msSqlContainer = factory.GetSqlContainer();

         var columnMetadata = await msSqlContainer.GetMetadataAsync(tableName);

         var command = new StringBuilder($"USE {SqlExtensions.SqlOptions.Database}; SELECT * FROM {tableName};");
         var dataResult = await msSqlContainer.ExecMsSqlScriptAsync(command.ToString());

         var rows = ParseSqlResponseToJsonArray(dataResult.Stdout, columnMetadata);
         return rows.Select(row => JsonSerializer.Deserialize<TObject>(row, _jsonOptions)!).ToList();
      }

      public static async Task<List<TObject>> GetMsSqlCollectionWithBaseAsync<TObject>(
        this IApplicationFactory factory,
        string tableName,
        string baseTableName,
        string tableKey,
        string baseKey)
        where TObject : class
      {
         var msSqlContainer = factory.GetSqlContainer();

         var columnMetadata = await msSqlContainer.GetMetadataAsync(tableName);
         var baseColumnMetadata = await msSqlContainer.GetMetadataAsync(baseTableName);
         columnMetadata.AddRange(baseColumnMetadata);

         var command = new StringBuilder($"USE {SqlExtensions.SqlOptions.Database}; SELECT * FROM {tableName} AS FST INNER JOIN {baseTableName} AS SND ON FST.{tableKey} = SND.{baseKey} ;");
         var dataResult = await msSqlContainer.ExecMsSqlScriptAsync(command.ToString());

         var rows = ParseSqlResponseToJsonArray(dataResult.Stdout, columnMetadata);
         return rows.Select(row => JsonSerializer.Deserialize<TObject>(row, _jsonOptions)!).ToList();
      }

      private static List<string> ParseSqlResponseToJsonArray(string response, List<(string ColumnName, string DataType)> columnMetadata)
      {
         var lines = response.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

         var headerLine = lines[0];
         var separatorLine = lines[1];
         var dataLines = lines.Skip(2).Take(lines.Length - 3);

         var columns = ParseColumns(headerLine, separatorLine);
         var result = new List<string>();

         foreach (var dataLine in dataLines)
         {
            result.Add(GetRow(columns, dataLine, columnMetadata));
         }

         return result;
      }

      private static List<(string ColumnName, string DataType)> ParseColumnMetadata(string metadataResponse)
      {
         return metadataResponse
           .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
           .Skip(1)
           .Select(line =>
           {
              var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
              return (ColumnName: parts[0], DataType: parts[1]);
           })
           .ToList();
      }

      private static string ParseSqlResponseToJson(string response, List<(string ColumnName, string DataType)> columnMetadata)
      {
         var lines = response.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

         var headerLine = lines[0];
         var separatorLine = lines[1];
         var dataLine = lines[2];

         var columns = ParseColumns(headerLine, separatorLine);

         return GetRow(columns, dataLine, columnMetadata);
      }

      private static string GetRow(List<(string ColumnName, int StartIndex, int Length)> columns, string dataLine, List<(string ColumnName, string DataType)> columnMetadata)
      {
         var result = new Dictionary<string, object?>();

         foreach (var (ColumnName, StartIndex, Length) in columns)
         {
            var value = dataLine.Substring(StartIndex, Length).Trim();
            var metadata = columnMetadata.FirstOrDefault(c => c.ColumnName == ColumnName);
            result[ColumnName] = ConvertToType(value, metadata.DataType);
         }

         return JsonSerializer.Serialize(result, _jsonOptions);
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

      private static object? ConvertToType(string value, string dataType)
      {
         return dataType.ToLower() switch
         {
            "int" => value is null || value.Equals("NULL") ? (int?)null : int.TryParse(value, CultureInfo.InvariantCulture, out var intValue) ? intValue : 0,
            "bigint" => value is null || value.Equals("NULL") ? (long?)null : long.TryParse(value, CultureInfo.InvariantCulture, out var longValue) ? longValue : 0L,
            "decimal" => value is null || value.Equals("NULL") ? (decimal?)null : decimal.TryParse(value, CultureInfo.InvariantCulture, out var decimalValue) ? decimalValue : 0M,
            "numeric" => value is null || value.Equals("NULL") ? (decimal?)null : decimal.TryParse(value, CultureInfo.InvariantCulture, out var decimalValue) ? decimalValue : 0M,
            "float" => value is null || value.Equals("NULL") ? (float?)null : float.TryParse(value, CultureInfo.InvariantCulture, out var floatValue) ? floatValue : 0F,
            "real" => value is null || value.Equals("NULL") ? (float?)null : float.TryParse(value, CultureInfo.InvariantCulture, out var floatValue) ? floatValue : 0F,
            "double" => value is null || value.Equals("NULL") ? (double?)null : double.TryParse(value, CultureInfo.InvariantCulture, out var doubleValue) ? doubleValue : 0D,
            "bit" => value is null || value.Equals("NULL") ? (bool?)null : (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase)),
            "uniqueidentifier" => value is null || value.Equals("NULL") ? (Guid?)null : Guid.TryParse(value, CultureInfo.InvariantCulture, out var guidValue) ? guidValue : Guid.Empty,
            "date" => value is null || value.Equals("NULL") ? (DateOnly?)null : DateOnly.TryParse(value, CultureInfo.InvariantCulture, out var dateOnlyValue) ? dateOnlyValue : default,
            "time" => value is null || value.Equals("NULL") ? (TimeOnly?)null : TimeOnly.TryParse(value, CultureInfo.InvariantCulture, out var timeOnlyValue) ? timeOnlyValue : default,
            "datetime" or "smalldatetime" => value is null || value.Equals("NULL") ? (DateTime?)null : DateTime.TryParse(value, CultureInfo.InvariantCulture, out var dateValue) ? dateValue : default,
            _ => value is null || value.Equals("NULL") ? null : value
         };
      }

      private static IContainer GetSqlContainer(this IApplicationFactory factory)
        => factory.Containers.First(x => x.Key == SqlOptions.ContainerName).Value;

      private static async Task<List<(string ColumnName, string DataType)>> GetMetadataAsync(this IContainer container, string tableName)
      {
         var columnMetadataQuery = $@"USE {SqlExtensions.SqlOptions.Database}; SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}';";

         var columnMetadataResult = await container.ExecMsSqlScriptAsync(columnMetadataQuery);
         return ParseColumnMetadata(columnMetadataResult.Stdout);
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
