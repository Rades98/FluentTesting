using Samples.KafkaConsumerStorage.Contracts;

namespace Samples.KafkaConsumerStorage.Services
{
	public interface ISomeService
	{
		public Task<bool> HandleAndStoreAvroData(IncomingKafkaMessage message, CancellationToken cancellationToken);
	}
}
