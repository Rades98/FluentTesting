using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using FluentTesting.Common.Options;

namespace FluentTesting.Common.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder SetWaitStrategy(this ContainerBuilder builder, int port, Options.WaitStrategy? waitStrategy)
        {
            builder
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(port, w =>
                    {
                        if (waitStrategy is null)
                        {
                            return;
                        }

                        if (waitStrategy.RetryCount is ushort retries)
                        {
                            w.WithRetries(retries);
                        }

                        if (waitStrategy.IntervalSeconds is int interval)
                        {
                            w.WithInterval(TimeSpan.FromSeconds(interval));
                        }

                        if (waitStrategy.TimeoutSeconds is int timeout)
                        {
                            w.WithTimeout(TimeSpan.FromSeconds(timeout));
                        }
                    }));

            return builder;
        }

        public static ContainerBuilder SetContainer(this ContainerBuilder builder, ContainerConfig? containerConfig)
        {
            if (containerConfig is not null && containerConfig is { } config)
            {
                builder.WithCreateParameterModifier(p =>
                {
                    p.HostConfig = p.HostConfig ?? new HostConfig();
                    p.HostConfig.Memory = config.MemoryMb * 1024 * 1024;
                    p.HostConfig.NanoCPUs = (long)(config.CPUs * 1_000_000_000);
                });
            }

            return builder;
        }
    }
}
