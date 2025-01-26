using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Azurite.Options;
using FluentTesting.Common.Extensions;

namespace FluentTesting.Azurite.Containers
{
	internal static class AzuriteContainerUtils
	{
		internal const int BlobPort = 10000;
		internal const int QueuePort = 10001;
		internal const int TablePort = 10002;

		internal static IContainer GetAzuriteContainer(INetwork network, AzuriteOptions azuriteOpts, bool useProxiedImages)
			=> new ContainerBuilder()
					.WithNetwork(network)
					.WithCleanUp(true)
					.WithNetworkAliases("azurite")
					.WithName($"TestContainers-Azurite-{Guid.NewGuid()}")
					.WithImage("azure-storage/azurite".GetProxiedImagePath(useProxiedImages, "mcr.microsoft.com"))
					.WithEnvironment("AZURITE_ACCOUNTS", "devstoreaccount1:devstoreaccount1")
					.WithPortBinding(azuriteOpts.BlobPort ?? BlobPort, BlobPort)
					.WithPortBinding(azuriteOpts.QueuePort ?? QueuePort, QueuePort)
					.WithPortBinding(azuriteOpts.TablePort ?? TablePort, TablePort)
					.WithWaitStrategy(Wait
						.ForUnixContainer()
						.UntilPortIsAvailable(BlobPort)
						.UntilPortIsAvailable(QueuePort)
						.UntilPortIsAvailable(TablePort))
					.Build();

		internal static IContainer GetAzureCLIContainer(INetwork network, bool useProxiedImages)
			=> new ContainerBuilder()
				.WithNetwork(network)
				.WithCleanUp(true)
				.WithName($"AzureCli-{Guid.NewGuid()}")
				.WithImage("azure-cli".GetProxiedImagePath(useProxiedImages, "mcr.microsoft.com"))
				.WithCommand("/bin/sh", "-c", "while true; do sleep 1000; done")
				.Build();

		internal static IContainer GetAzureExplorerContainer(INetwork network, IContainer azuriteContainer, AzuriteOptions azuriteOpts, bool useProxiedImages)
			=> new ContainerBuilder()
					.WithNetwork(network)
					.DependsOn(azuriteContainer)
					.WithName($"TestContainers-AzureStorageExplorer-{Guid.NewGuid()}")
					.WithImage("sebagomez/azurestorageexplorer".GetProxiedImagePath(useProxiedImages))
					.WithEnvironment("AZURE_STORAGE_CONNECTIONSTRING", GetConnectionString(azuriteContainer, azuriteOpts, "azurite"))
					.WithPortBinding(azuriteOpts.GuiPort, 8080)
					.WithCleanUp(true)
					.Build();

		internal static string GetConnectionString(IContainer container, AzuriteOptions azuriteOptions, string? networkAlias = null)
		{
			var blob = $"http://{networkAlias ?? container.Hostname}:{container.GetMappedPublicPort(BlobPort)}/{azuriteOptions.DefaultUserName}";
			var queue = $"http://{networkAlias ?? container.Hostname}:{container.GetMappedPublicPort(QueuePort)}/{azuriteOptions.DefaultUserName}";
			var table = $"http://{networkAlias ?? container.Hostname}:{container.GetMappedPublicPort(TablePort)}/{azuriteOptions.DefaultUserName}";

			var properties = new Dictionary<string, string>
			{
				{ "DefaultEndpointsProtocol", "http" },
				{ "AccountName", azuriteOptions.DefaultUserName },
				{ "AccountKey", azuriteOptions.DefaultPassword },
				{ "BlobEndpoint", blob },
				{ "QueueEndpoint", queue },
				{ "TableEndpoint", table },
			};

			return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
		}
	}
}
