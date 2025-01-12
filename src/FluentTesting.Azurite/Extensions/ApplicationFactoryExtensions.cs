using DotNet.Testcontainers.Containers;
using FluentTesting.Azurite.Options;
using FluentTesting.Common.Interfaces;
using System.Text.Json;

namespace FluentTesting.Azurite.Extensions
{
	public static class ApplicationFactoryExtensions
	{
		private static JsonSerializerOptions _jsonSerOpts = new()
		{
			PropertyNameCaseInsensitive = true,
			Converters = { new BlobDataConverter() }
		};

		/// <summary>
		/// Get informations about blob
		/// </summary>
		/// <param name="factory">factory</param>
		/// <param name="containerName">Azure blob container name</param>
		/// <param name="fileName">Blob name</param>
		/// <returns></returns>
		public static async Task<BlobData?> GetBlobInformationsAsync(this IApplicationFactory factory, string containerName, string fileName)
		{
			var container = factory.GetAzureCliContainer();

			var connectionString = GetBlobConnectionString(AzuriteExtensions.AzuriteOptions);

			var res = await container.ExecAsync(["/bin/bash", "-c", $"az storage blob show --container-name {containerName} --name {fileName} --connection-string '{connectionString}'"]);

			if (res.ExitCode == 0 && !string.IsNullOrEmpty(res.Stdout))
			{
				return JsonSerializer.Deserialize<BlobData>(res.Stdout, _jsonSerOpts)!;
			}

			return null;
		}

		private static string GetBlobConnectionString(AzuriteOptions azuriteOptions)
		{
			var blob = $"http://azurite:10000/{azuriteOptions.DefaultUserName}";

			var properties = new Dictionary<string, string>
			{
				{ "DefaultEndpointsProtocol", "http" },
				{ "AccountName", azuriteOptions.DefaultUserName },
				{ "AccountKey", azuriteOptions.DefaultPassword },
				{ "BlobEndpoint", blob },
			};

			return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
		}

		private static IContainer GetAzureCliContainer(this IApplicationFactory factory)
			=> factory.Containers.First(x => x.Key == AzuriteOptions.AzureCliContainerName).Value;

		private static IContainer GetAzuriteContainer(this IApplicationFactory factory)
			=> factory.Containers.First(x => x.Key == AzuriteOptions.AzureCliContainerName).Value;
	}
}
