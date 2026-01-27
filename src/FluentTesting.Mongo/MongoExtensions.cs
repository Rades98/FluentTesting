using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Common.Providers;
using FluentTesting.Mongo.Container;
using FluentTesting.Mongo.Options;
using Microsoft.Extensions.Configuration;

namespace FluentTesting.Mongo;

public static class MongoExtensions
{
    static readonly MongoDbOptions mongoDbOptions = new();

    /// <summary>
    /// Use MongoDb container with initial seed, configuration and (optional) custom options.
    /// </summary>
    public static IApplicationFactoryBuilder UseMongo(
        this IApplicationFactoryBuilder builder,
        string seed,
        Action<ConfigurationBuilder, MongoContainerSettings> configuration,
        Action<MongoDbOptions>? customOptions = null)
    {
        customOptions ??= _ => { };

        customOptions.Invoke(mongoDbOptions);

        var (MongoContainer, MongoClientContainer, MongoNetwork) = CreateMongo(seed, builder.UseProxiedImages);

        builder.Containers.TryAdd(nameof(MongoContainer), MongoContainer);

        if (MongoClientContainer is not null)
        {
            builder.Containers.TryAdd(nameof(MongoClientContainer), MongoClientContainer);
        }

        builder.Networks.TryAdd(nameof(MongoNetwork), MongoNetwork);

        builder.Builders.Add(
            configurationBuilder => configuration.Invoke(
                configurationBuilder,
                new MongoContainerSettings(
                        [MongoContainer.Container.Hostname],
                        MongoContainer.Container.GetMappedPublicPort(MongoContainerUtils.MongoDbPort),
                        mongoDbOptions.Username,
                        mongoDbOptions.Password,
                        mongoDbOptions.DatabaseName
                        )
                )
            );

        return builder;
    }

    /// <summary>
    /// Creates and runs MongoDb container with specified seed.
    /// </summary>
    static (ContainerActionPair MongoContainer, ContainerActionPair? MongoClientContainer, INetwork MongoNetwork) CreateMongo(string seed, bool useProxiedImages)
    {
        var network = NetworkProvider.GetBasicNetwork();

        IContainer? clientContainer = null;

        var container = MongoContainerUtils.GetMongoContainer(network, mongoDbOptions, useProxiedImages);

        ContainerActionPair mongoContainerPair = new(container, async mongoContainer =>
        {
            var result = await mongoContainer.EnsureContainerAsync(cnt => MongoContainerUtils.ExecMongoScriptAsync(cnt, seed));

            if (!string.IsNullOrEmpty(seed) && result.ExitCode != 0)
            {
                throw new Exception("Mongo seed failed: " + result.Stderr);
            }

            return result;
        });

        ContainerActionPair? clientContainerPair = null;

        if (System.Diagnostics.Debugger.IsAttached && mongoDbOptions.RunAdminTool)
        {
            clientContainer = MongoContainerUtils.GetMongoExpressContainer(network, useProxiedImages);

            clientContainerPair = new(clientContainer, cnt => cnt.EnsureContainerAsync());
        }

        return (mongoContainerPair, clientContainerPair, network);
    }
}