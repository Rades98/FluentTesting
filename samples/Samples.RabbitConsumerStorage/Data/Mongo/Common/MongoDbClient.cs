using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Samples.RabbitConsumerStorage.Data.Mongo.Common;

internal class MongoDbClient(IOptions<MongoConfigurationOptions> mongoConfig)
	: MongoClient(
		new MongoClientSettings
			{
				AllowInsecureTls = false,
				ApplicationName = "Samples.RabbitConsumerStorage",
				Credential =
					MongoCredential.CreateCredential(
						mongoConfig.Value.DatabaseName,
						mongoConfig.Value.Username,
						mongoConfig.Value.Password),
				Servers = mongoConfig.Value.Hosts.Select(
					host => new MongoServerAddress(host, mongoConfig.Value.Port)),
				UseTls = mongoConfig.Value.UseTls
			})
{
}
