using Alza.Client.Kafka.Publisher;
using Alza.KafkaTest;
using Samples.KafkaConsumerPublisher.Contracts;

namespace Samples.KafkaConsumerPublisher.Services
{
	internal class AvroHandlingService(IKafkaPublisher kafkaPublisher) : IAvroHandlingService
	{
		public Task<bool> EnrichAndPublishChangeEvent(IncomingKafkaMessage incomingKafkaMessage, CancellationToken cancellationToken)
			=> kafkaPublisher
				.PublishAsync("outgoing.topic",
					new SomeOtherAvro()
					{
						SomeDate = incomingKafkaMessage.DateTime,
						SomeString = incomingKafkaMessage.String
					},
					cancellationToken)
				.ContinueWith(task => task.IsCompletedSuccessfully);
	}
}
