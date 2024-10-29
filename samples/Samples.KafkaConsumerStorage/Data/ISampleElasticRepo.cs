using Samples.KafkaConsumerStorage.Contracts;

namespace Samples.KafkaConsumerStorage.Data
{
	public interface ISampleElasticRepo
	{
		Task<bool> IndexData(IncomingKafkaMessage message, CancellationToken cancellationToken);
	}
}
