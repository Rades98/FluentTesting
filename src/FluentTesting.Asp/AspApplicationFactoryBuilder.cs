using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FluentTesting.Asp
{
	public class AspApplicationFactoryBuilder<Program> : WebApplicationFactory<Program>, IApplicationFactoryBuilder
		where Program : class
	{
		public ConcurrentDictionary<string, ContainerActionPair> Containers { get; } = new();
		public ConcurrentDictionary<string, INetwork> Networks { get; } = new();
		public ConcurrentBag<Action<ConfigurationBuilder>> Builders { get; } = [];

		public ConcurrentBag<Action<IServiceCollection, IConfiguration>> AppServices { get; } = [];

		public bool UseProxiedImages { get; set; } = false;

		private Action<IServiceCollection, IConfiguration>? services;
		private Action<ConfigurationBuilder>? configurationBuilder;
		private Action<HttpRequestHeaders>? clientHeaders;
		private string environmentName = "IntegrationTests";

		private Regex? assertationRegex = null;

		/// <inheritdoc/>
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			IConfiguration? configuration = null;

			builder.UseEnvironment(environmentName);

			builder.ConfigureAppConfiguration((context, builder) =>
			{
				builder.Sources.Clear();
				var confBuilder = new ConfigurationBuilder();

				foreach (var otherBuilder in Builders)
				{
					otherBuilder?.Invoke(confBuilder);
				}

				configurationBuilder?.Invoke(confBuilder);

				configuration = confBuilder.Build();

				builder.AddConfiguration(configuration);

			});

			builder.ConfigureTestServices(services =>
			{
				foreach (var otherServices in AppServices)
				{
					otherServices?.Invoke(services, configuration!);
				}

				this.services?.Invoke(services, configuration!);
			});

			builder.UseTestServer();
		}

		protected override void ConfigureClient(HttpClient client)
		{
			base.ConfigureClient(client);

			if (clientHeaders is not null)
			{
				clientHeaders.Invoke(client.DefaultRequestHeaders);
			}
		}

		/// <inheritdoc/>
		public override async ValueTask DisposeAsync()
		{
			foreach (var container in Containers)
			{
				await container.Value.Container.StopAsync().ConfigureAwait(false);
				await container.Value.Container.DisposeAsync().ConfigureAwait(false);
			}

			foreach (var network in Networks)
			{
				await network.Value.DisposeAsync().ConfigureAwait(false);
			}

			GC.SuppressFinalize(this);

			await base.DisposeAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Build 
		/// </summary>
		/// <returns></returns>
		public IApplicationFactory Build()
		{
			if (assertationRegex is null)
			{
				var projectName = Assembly.GetCallingAssembly().GetName().Name!;
				assertationRegex = new(@$".*{projectName.Replace(",", "\\.")}[\\\/]+(.*?)[\\\/](?:(?![\\\/]).)*$", RegexOptions.Compiled);
			}

			return new AspApplicationFactory(Services, CreateDefaultClient(), Containers, assertationRegex);
		}

		/// <summary>
		/// Register test specific services, all your previously registered services from startup will stay registered
		/// </summary>
		/// <param name="services">services collection and configuration pair</param>
		/// <returns></returns>
		public AspApplicationFactoryBuilder<Program> RegisterServices(Action<IServiceCollection, IConfiguration> services)
		{
			this.services = services;
			return this;
		}

		/// <summary>
		/// Use configuration - Configure options (all previous providers are deleted, so appsettings.json etc will not work here)
		/// </summary>
		/// <param name="config">Configuration builder</param>
		/// <returns></returns>
		public AspApplicationFactoryBuilder<Program> UseConfiguration(Action<ConfigurationBuilder> config)
		{
			configurationBuilder = config;
			return this;
		}

		/// <summary>
		/// Use http client headers
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public AspApplicationFactoryBuilder<Program> UseClientHeaders(Action<HttpRequestHeaders> headers)
		{
			clientHeaders = headers;
			return this;
		}

		/// <summary>
		/// Use specific environmnet - by default env name is IntegrationTests
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public AspApplicationFactoryBuilder<Program> UseEnvironment(string environment)
		{
			environmentName = environment;
			return this;
		}

		/// <summary>
		/// Set assertation regex for json assertations
		/// </summary>
		/// <param name="regex"></param>
		/// <returns></returns>
		public AspApplicationFactoryBuilder<Program> SetAssertionRegex(string regex)
		{
			assertationRegex = new(regex, RegexOptions.Compiled);

			return this;
		}

		/// <summary>
		/// Set test project name for assertion regex
		/// </summary>
		/// <param name="regex"></param>
		/// <returns></returns>
		public AspApplicationFactoryBuilder<Program> SetTestProjectName(string projectName)
		{
			assertationRegex = new(@$".*{projectName.Replace(",", "\\.")}[\\\/]+(.*?)[\\\/](?:(?![\\\/]).)*$", RegexOptions.Compiled);

			return this;
		}
	}
}
