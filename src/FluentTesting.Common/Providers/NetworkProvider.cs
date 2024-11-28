using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;

namespace FluentTesting.Common.Providers
{
    public static class NetworkProvider
    {
        public static INetwork GetBasicNetwork()
            => new NetworkBuilder()
                    .WithName(Guid.NewGuid().ToString("D"))
                        .Build();
    }
}
