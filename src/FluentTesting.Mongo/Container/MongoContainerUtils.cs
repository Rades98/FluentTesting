using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Extensions;
using FluentTesting.Mongo.Options;
using System.Text;

namespace FluentTesting.Mongo.Container
{
    internal static class MongoContainerUtils
    {
        internal const int MongoDbPort = 27017;

        internal static IContainer GetMongoContainer(INetwork network, MongoDbOptions mongoDbOptions, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("mongo:6.0".GetProxiedImagePath(useProxiedImages))
                .WithCleanUp(true)
                .WithNetwork(network)
                .WithNetworkAliases("mongo")
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "admin")
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "admin")
                .WithPortBinding(mongoDbOptions.Port ?? MongoDbPort, MongoDbPort)
                .WithName($"TestContainers-MongoDb-{Guid.NewGuid()}")
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(MongoDbPort))
                .Build();

        internal static IContainer GetMongoExpressContainer(INetwork network, bool useProxiedImages)
            => new ContainerBuilder()
                .WithImage("mongo-express:1.0.2-18".GetProxiedImagePath(useProxiedImages))
                .WithCleanUp(true)
                .WithNetwork(network)
                .WithName($"TestContainers-MongoExpress-{Guid.NewGuid()}")
                .WithEnvironment("ME_CONFIG_MONGODB_SERVER", "mongo")
                .WithEnvironment("ME_CONFIG_MONGODB_ADMINUSERNAME", "admin")
                .WithEnvironment("ME_CONFIG_MONGODB_ADMINPASSWORD", "admin")
                .WithPortBinding(9966, 8081)
                .Build();

        internal static async Task<ExecResult> ExecMongoScriptAsync(IContainer container, string scriptContent, CancellationToken ct = default)
        {
            await Task.Delay(2000, ct).ConfigureAwait(false);

            var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

            await container.CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
                .ConfigureAwait(false);

            var whichMongoDbShell = await container.ExecAsync(["which", "mongosh"], ct)
                .ConfigureAwait(false);

            var command = new[]
            {
                whichMongoDbShell.ExitCode == 0 ? "mongosh" : "mongo",
                "--username", "admin",
                "--password", "admin",
                "--quiet",
                "--eval",
                $"load('{scriptFilePath}')",
            };

            return await container.ExecAsync(command, ct)
                .ConfigureAwait(false);
        }
    }
}
