using FluentTesting.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace FluentTesting.Common.Abstraction
{
    /// <summary>
    /// Application factory base object
    /// </summary>
    /// <param name="host"></param>
    public class ApplicationFactory(IHost host) : IApplicationFactory, IAsyncDisposable
    {
        private Action DisposeActions = () => { };

        /// <inheritdoc/>
        public IHost Host { get; } = host;

        /// <inheritdoc/>
        public IServiceProvider Services => Host.Services;

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
    }
}
