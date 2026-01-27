using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using Xunit;

namespace FluentTesting.Common.Abstraction
{
    /// <summary>
    /// Application factory base object
    /// </summary>
    /// <param name="host"></param>
    public class ApplicationFactory(IHost host, ConcurrentDictionary<string, ContainerActionPair> containers) : IApplicationFactory, IAsyncLifetime
    {
        private Action DisposeActions = () => { };

        /// <inheritdoc/>
        public IHost Host { get; } = host;

        /// <inheritdoc/>
        public IServiceProvider Services => Host.Services;

        public ConcurrentDictionary<string, ContainerActionPair> Containers => containers;

        /// <inheritdoc/>
        public void AppendDisposeAction(Action disposeAction)
        {
            DisposeActions += disposeAction;
        }

        /// <summary>
        /// Wait for shutdown
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns></returns>
        public Task WaitForShutdownAsync(CancellationToken token)
            => Host.WaitForShutdownAsync(token);

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            DisposeActions.Invoke();

            return ValueTask.CompletedTask;
        }

        public async ValueTask InitializeAsync()
        {
            foreach (var va in containers.Values)
            {
                await va.Ensure.Invoke(va.Container);
            }
        }
    }
}
