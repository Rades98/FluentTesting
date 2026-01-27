using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Azurite.Containers;
using FluentTesting.Azurite.Options;
using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using Microsoft.Extensions.Configuration;

namespace FluentTesting.Azurite
{
    public static class AzuriteExtensions
    {
        internal static readonly AzuriteOptions AzuriteOptions = new();

        public static IApplicationFactoryBuilder UseAzurite(
            this IApplicationFactoryBuilder builder,
            Action<ConfigurationBuilder, AzuriteContainerSettings> configuration,
            Action<AzuriteOptions>? customOptions = null)
        {
            customOptions ??= _ => { };

            customOptions.Invoke(AzuriteOptions);

            var (azuriteContainer, azureCLIContainer, blobNetwork, azuriteClientContainer) = CreateBlob(builder.UseProxiedImages);

            if (azuriteClientContainer is not null)
            {
                builder.Containers.TryAdd(nameof(azuriteClientContainer), azuriteClientContainer);
            }

            builder.Containers.TryAdd(AzuriteOptions.AzuriteContainerName, azuriteContainer);
            builder.Containers.TryAdd(AzuriteOptions.AzureCliContainerName, azureCLIContainer);

            builder.Networks.TryAdd(nameof(blobNetwork), blobNetwork);

            builder.Builders.Add(cknfBuilder => configuration.Invoke(cknfBuilder, new(AzuriteContainerUtils.GetConnectionString(azuriteContainer.Container, AzuriteOptions))));

            return builder;
        }

        private static (ContainerActionPair AzuriteContainer, ContainerActionPair AzureCliContainer, INetwork BlobNetwork, ContainerActionPair? ClientContainer) CreateBlob(bool useProxiedImages)
        {
            var network = NetworkProvider.GetBasicNetwork();

            IContainer? clientContainer = null;

            var azuriteContainer = AzuriteContainerUtils.GetAzuriteContainer(network, AzuriteOptions, useProxiedImages);

            var azureCliContainer = AzuriteContainerUtils.GetAzureCLIContainer(network, AzuriteOptions, useProxiedImages);


            ContainerActionPair? azureExplorerContainer = null;

            if (System.Diagnostics.Debugger.IsAttached && AzuriteOptions.RunAdminTool)
            {
                clientContainer = AzuriteContainerUtils.GetAzureExplorerContainer(network, azuriteContainer, AzuriteOptions, useProxiedImages);


                azureExplorerContainer = new(clientContainer, cont => cont.EnsureContainerAsync());
            }

            ContainerActionPair azuriteContainerPair = new(azuriteContainer, async cont =>
            {
                return await cont.EnsureContainerAsync();
            });

            ContainerActionPair azuriteCliContainerPair = new(azureCliContainer, async cont =>
            {
                var res = await cont.EnsureContainerAsync(async container =>
                {
                    var connectionString = AzuriteContainerUtils.GetConnectionString(cont, AzuriteOptions, "azurite", true);

                    var results = new List<ExecResult>()
                    {
                        new("", "", 0)
                    };

                    foreach (var containerSeed in AzuriteOptions.BlobSeed)
                    {
                        results.Add(await container.ExecAsync(["/bin/bash", "-c", $"az storage container create -n '{containerSeed.Name}' --connection-string '{connectionString}'"]));

                        foreach (var file in containerSeed.Files)
                        {
                            var fileName = file.Name ?? $"{Path.GetFileName(file.Path)}";
                            var filePath = $"/blob/{containerSeed.Name}/{Path.GetFileName(file.Path)}";

                            await container.CopyAsync(file.Path, $"/blob/{containerSeed.Name}");

                            var result = await container.ExecAsync(["ls", "-l", $"/blob/{containerSeed.Name}"]);

                            results.Add(await container.ExecAsync(["/bin/bash", "-c", $"az storage blob upload --container-name '{containerSeed.Name}' --name '{fileName}' --file '{filePath}' --connection-string '{connectionString}'"]));
                        }
                    }

                    return results.Any(x => x.ExitCode != 0) ? results.First(x => x.ExitCode != 0) : results.First();
                });

                if (res.ExitCode != 0)
                {
                    throw new Exception("Azurite initialisation failed: " + res.Stderr);
                }

                return res;
            });

            return (azuriteContainerPair, azuriteCliContainerPair, network, azureExplorerContainer);
        }

    }
}
