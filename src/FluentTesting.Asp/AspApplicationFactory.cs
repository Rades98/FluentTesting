using FluentTesting.Common.Interfaces;
using Microsoft.AspNetCore.TestHost;
using System.Text.RegularExpressions;

namespace FluentTesting.Asp
{
    /// <summary>
    /// ASP Application factory
    /// </summary>
    public class AspApplicationFactory(IServiceProvider serviceProvider, HttpClient client, WebSocketClient wsClient, Regex? assertationRegex = null)
        : IApplicationFactory, IAsyncDisposable
    {
        /// <inheritdoc/>
        private Action DisposeActions = () => { };

        /// <inheritdoc/>
        public IServiceProvider Services => serviceProvider;

        public Regex? AssertionRegex => assertationRegex;

        /// <summary>
        /// Http client configured with base route of application
        /// </summary>
        public HttpClient Client => client;

        public WebSocketClient WebSocketClient => wsClient;

        /// <inheritdoc/>
        public void AppendDisposeAction(Action disposeAction)
        {
            DisposeActions += disposeAction;
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            DisposeActions.Invoke();

            return ValueTask.CompletedTask;
        }
    }
}
