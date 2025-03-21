﻿using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Interfaces;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace FluentTesting.Asp
{
    /// <summary>
    /// ASP Application factory
    /// </summary>
    public class AspApplicationFactory(IServiceProvider serviceProvider, HttpClient client, ConcurrentDictionary<string, IContainer> containers, Regex? assertationRegex = null)
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

        public ConcurrentDictionary<string, IContainer> Containers => containers;

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
