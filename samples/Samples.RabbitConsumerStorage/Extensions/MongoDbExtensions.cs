using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using Samples.RabbitConsumerStorage.Contracts;
using Samples.RabbitConsumerStorage.Data.Mongo;
using Samples.RabbitConsumerStorage.Data.Mongo.Common;

namespace Samples.RabbitConsumerStorage.Extensions;

public static class MongoDbExtensions
{
	public static IServiceCollection AddMongoDbClient(this IServiceCollection services)
	{
		BsonClassMap.RegisterClassMap<SampleMongoModel>(classMap =>
			{
				classMap.AutoMap();
				classMap.SetDiscriminator("SampleMongoModel");
				classMap.MapIdProperty(c => c.Id);
			});

		services.AddSingleton<IMongoClient, MongoDbClient>();

		return services;
	}
}