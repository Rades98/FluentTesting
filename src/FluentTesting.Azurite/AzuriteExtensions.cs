using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Azurite.Containers;
using FluentTesting.Azurite.Options;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using Microsoft.Extensions.Configuration;

namespace FluentTesting.Azurite
{
    internal static class AzuriteExtensions
    {
        private static readonly AzuriteOptions azuriteOptions = new();

        public static IApplicationFactoryBuilder UseAzurite(
            this IApplicationFactoryBuilder builder,
            Action<ConfigurationBuilder, AzuriteContainerSettings> configuration,
            Action<AzuriteOptions>? customOptions = null)
        {
            customOptions ??= _ => { };

            customOptions.Invoke(azuriteOptions);

            var (azuriteContainer, blobNetwork, azuriteClientContainer) = CreateBlob(builder.UseProxiedImages);

            if (azuriteClientContainer is not null)
            {
                builder.Containers.TryAdd(nameof(azuriteClientContainer), azuriteClientContainer);
            }

            builder.Containers.TryAdd(nameof(azuriteContainer), azuriteContainer);

            builder.Networks.TryAdd(nameof(blobNetwork), blobNetwork);

            builder.Builders.Add(cknfBuilder => configuration.Invoke(cknfBuilder, new(AzuriteContainerUtils.GetConnectionString(azuriteContainer, azuriteOptions))));

            return builder;
        }

        private static (IContainer AzuriteContainer, INetwork BlobNetwork, IContainer? ClientContainer) CreateBlob(bool useProxiedImages)
        {
            var network = NetworkProvider.GetBasicNetwork();

            IContainer? clientContainer = null;

            var azuriteContainer = AzuriteContainerUtils.GetAzuriteContainer(network, azuriteOptions, useProxiedImages);

            azuriteContainer.EnsureContainer();

            if (System.Diagnostics.Debugger.IsAttached && azuriteOptions.RunAdminTool)
            {
                clientContainer = AzuriteContainerUtils.GetAzureExplorerContainer(network, azuriteContainer, azuriteOptions, useProxiedImages);

                clientContainer.EnsureContainer();
            }

            return (azuriteContainer, network, clientContainer);
        }
    }
}
