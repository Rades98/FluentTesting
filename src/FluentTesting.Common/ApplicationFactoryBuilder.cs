using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentTesting.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using Testing.Common.Abstraction;

namespace FluentTesting.Common
{
    /// <summary>
    /// Application factory builder
    /// </summary>
    /// <typeparam name="Program"></typeparam>
    public sealed class ApplicationFactoryBuilder<Program> : WebApplicationFactory<Program>, IApplicationFactoryBuilder
        where Program : class
    {
        public ConcurrentDictionary<string, IContainer> Containers { get; } = new();
        public ConcurrentDictionary<string, INetwork> Networks { get; } = new();
        public ConcurrentBag<Action<ConfigurationBuilder>> Builders { get; } = [];

        public ConcurrentBag<Action<IServiceCollection, IConfiguration>> AppServices { get; } = [];

        public bool UseProxiedImages { get; set; } = false;

        private Action<IWebHostBuilder>? internalConfiguration;
        private Action<IServiceCollection, IConfiguration>? services;
        private Action<ConfigurationBuilder>? configurationBuilder;
        private string environmentName = "IntegrationTests";

        /// <inheritdoc/>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            IConfiguration? configuration = null;

            internalConfiguration = new Action<IWebHostBuilder>(builder =>
            {
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
            });

            internalConfiguration(builder.Configure((_) => { }));
        }

        /// <inheritdoc/>
        public override async ValueTask DisposeAsync()
        {
            foreach (var container in Containers)
            {
                await container.Value.StopAsync().ConfigureAwait(false);
                await container.Value.DisposeAsync().ConfigureAwait(false);
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
            var host = Services.GetRequiredService<IHost>();

            return new ApplicationFactory(host);
        }

        /// <summary>
        /// Register test specific services, all your previously registered services from startup will stay registered
        /// </summary>
        /// <param name="services">services collection and configuration pair</param>
        /// <returns></returns>
        public ApplicationFactoryBuilder<Program> RegisterServices(Action<IServiceCollection, IConfiguration> services)
        {
            this.services = services;
            return this;
        }

        /// <summary>
        /// Use configuration - Configure options (all previous providers are deleted, so appsettings.json etc will not work here)
        /// </summary>
        /// <param name="config">Configuration builder</param>
        /// <returns></returns>
        public ApplicationFactoryBuilder<Program> UseConfiguration(Action<ConfigurationBuilder> config)
        {
            configurationBuilder = config;
            return this;
        }

        /// <summary>
        /// Use specific environmnet - by default env name is IntegrationTests
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public ApplicationFactoryBuilder<Program> UseEnvironment(string environment)
        {
            environmentName = environment;
            return this;
        }

        /// <summary>
        /// Not use proxied images - use with caution on local machine in case of lack of VPN
        /// </summary>
        public ApplicationFactoryBuilder<Program> NotUseProxiedImages()
        {
            Trace.WriteLine("DO NOT FORGET TO REMOVE THIS BEFORE PUSHING TO AzDo!!");

            UseProxiedImages = false;
            return this;
        }
    }
}
