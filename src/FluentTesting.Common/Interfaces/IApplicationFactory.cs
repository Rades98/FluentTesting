using DotNet.Testcontainers.Containers;
using System.Collections.Concurrent;

namespace FluentTesting.Common.Interfaces
{
    /// <summary>
    /// Application factory interface
    /// </summary>
    public interface IApplicationFactory
    {
        /// <summary>
        /// Services
        /// </summary>
        IServiceProvider Services { get; }

        public ConcurrentDictionary<string, IContainer> Containers { get; }

        /// <summary>
        /// Append dispose action - used to add disposable objects to collection in factory
        /// </summary>
        /// <param name="disposeAction">dispose action</param>
        void AppendDisposeAction(Action disposeAction);
    }
}
