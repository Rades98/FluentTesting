namespace Samples.RabbitConsumerStorage.Data.Mongo;

public interface ISampleMongoRepository
{
	Task InsertSampleModel(SampleMongoModel model, CancellationToken ct);
}