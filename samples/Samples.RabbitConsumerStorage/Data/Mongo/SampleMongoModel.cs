using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Samples.RabbitConsumerStorage.Data.Mongo;

public class SampleMongoModel
{
	[BsonId]
	public ObjectId Id { get; set; }

	public string? Text { get; set; }
	
	public DateTime Date { get; set; }
}