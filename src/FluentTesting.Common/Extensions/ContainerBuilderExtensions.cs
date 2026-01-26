using DotNet.Testcontainers.Builders;

namespace FluentTesting.Common.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder SetWaitStrategy(this ContainerBuilder builder, int port, Options.WaitStrategy? waitStrategy)
        {
            builder
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(port, w =>
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
    }
}
