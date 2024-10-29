using Samples.KafkaConsumerStorage.Contracts;
using Samples.KafkaConsumerStorage.Data;

namespace Samples.KafkaConsumerStorage.Services
{
	internal class SomeService(ISampleElasticRepo repo) : ISomeService
	{
		public Task<bool> HandleAndStoreAvroData(IncomingKafkaMessage message, CancellationToken cancellationToken)
			=> repo.IndexData(message, cancellationToken);
	}
}
