using Alza.BackendCore.Redis;
using Alza.BackendCore.Redis.Configuration;
using Alza.Client.Rabbit.Consumer;
using Alza.Client.Rabbit.Consumer.Configuration;
using Alza.Library.Configurations.System;
using Alza.Platform.Applications.BackgroundWorker;

using MediatR;

using Samples.RabbitConsumerStorage.Data.Mongo;
using Samples.RabbitConsumerStorage.Data.Mongo.Common;
using Samples.RabbitConsumerStorage.Data.Redis;
using Samples.RabbitConsumerStorage.Extensions;
using Samples.RabbitConsumerStorage.Services;

namespace Samples.RabbitConsumerStorage
{
	internal class Startup(IConfiguration configuration) : IBackgroundWorkerStartup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHostedService<Worker>();
			
			// rabbit consumer
			services.AddMediatR(typeof(Program));
			services.Configure<ConsumerConfiguration>(options => configuration.GetSection("rabbitConsumerConfiguration").Bind(options));
			services.AddBusConsumer();
			services.AddHostedService<ConsumerHostedService>();
			
			// mongo registration
			services.AddTransient<ISampleMongoRepository, SampleMongoRepository>();
			services.Configure<MongoConfigurationOptions>(options => configuration.GetSection("MongoConfigurationOptions").Bind(options));
			services.AddMongoDbClient();
			
			// redis registrations
			services.Configure<RedisStorageConfiguration>(options => configuration.GetSection("RedisStorageConfiguration").Bind(options));
			services.Configure<RedisConfigurationOptions>(options => configuration.GetSection("RedisStoreConfigurationOptions").Bind(options));
			services.AddSingleton<IRedisService, RedisService>();
			services.AddTransient<ISampleRedisRepository, SampleRedisRepository>();
			
			// services
			services.AddTransient<ISampleService, SampleService>();
		}
	}
}