using MongoDB.Driver;

namespace Samples.RabbitConsumerStorage.Data.Mongo;

public class SampleMongoRepository(IMongoClient mongoClient) : ISampleMongoRepository
{
	IMongoDatabase Database => mongoClient.GetDatabase("TestingSamplesDatabase");

	const string CollectionName = "SampleCollection";
	
	public async Task InsertSampleModel(SampleMongoModel model, CancellationToken ct)
	{
		var collection = Database.GetCollection<SampleMongoModel>(CollectionName);

		await collection.InsertOneAsync(model, new InsertOneOptions(), ct);
	}
}