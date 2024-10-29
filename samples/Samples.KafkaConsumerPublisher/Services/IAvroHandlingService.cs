using Samples.KafkaConsumerPublisher.Contracts;

namespace Samples.KafkaConsumerPublisher.Services
{
	public interface IAvroHandlingService
	{
		Task<bool> EnrichAndPublishChangeEvent(IncomingKafkaMessage incomingKafkaMessage, CancellationToken cancellationToken);
	}
}
