using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FluentTesting.Asp
{
    /// <summary>
    /// ASP Application factory
    /// </summary>
    public class AspApplicationFactory(IServiceProvider serviceProvider, HttpClient client, ConcurrentDictionary<string, ContainerActionPair> containers, Func<HttpClient> createClientDelegate, Regex? assertationRegex = null)
        : IApplicationFactory
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

        /// <summary>
        /// Get http client
        /// </summary>
        /// <returns></returns>
        public HttpClient GetClient() => createClientDelegate.Invoke();

        public ConcurrentDictionary<string, ContainerActionPair> Containers => containers;

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

        public async ValueTask InitializeAsync()
        {
            foreach (var va in containers.Values)
            {
                var res = await va.Ensure.Invoke(va.Container);
                Debug.WriteLine($"[Container Initialized] {va.Container.Name} - {res}");
            }
        }
    }
}
