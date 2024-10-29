using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Testing.Common.Interfaces
{
	/// <summary>
	/// Application factory builder interface
	/// </summary>
	public interface IApplicationFactoryBuilder
	{
		/// <summary>
		/// Containers
		/// </summary>
		public ConcurrentDictionary<string, IContainer> Containers { get; }

		/// <summary>
		/// Networks
		/// </summary>
		public ConcurrentDictionary<string, INetwork> Networks { get; }

		/// <summary>
		/// Builders
		/// </summary>
		public ConcurrentBag<Action<ConfigurationBuilder>> Builders { get; }

		/// <summary>
		/// Services
		/// </summary>
		public ConcurrentBag<Action<IServiceCollection, IConfiguration>> AppServices { get; }

		/// <summary>
		/// Build application
		/// </summary>
		/// <returns></returns>
		public IApplicationFactory Build();

		public bool UseProxiedImages { get; set; }
	}
}
