using Alza.Client.Kafka.Common.Interfaces;
using Alza.Client.Kafka.Consumer;
using Alza.Client.Kafka.Consumer.ConsumptionResult;
using Alza.KafkaTest;
using Samples.KafkaConsumerPublisher.Services;

namespace Samples.KafkaConsumerPublisher.MessageHandlers
{
	internal class SomeAvroMessageHandler(IAvroHandlingService service) : KafkaMessageHandler<SomeAwesomeAvro>
	{
		public override async Task<IKafkaConsumptionResult> Handle(SomeAwesomeAvro message, CancellationToken cancellationToken)
		{
			if (await service.EnrichAndPublishChangeEvent(new(message.SomeDate, message.SomeString), cancellationToken))
			{
				return KafkaConsumptionResult.Handle;
			}

			return KafkaConsumptionResult.Reject();
		}
	}
}
