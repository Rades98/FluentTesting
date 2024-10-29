using Alza.Client.Kafka.Common.Interfaces;
using Alza.Client.Kafka.Consumer;
using Alza.Client.Kafka.Consumer.ConsumptionResult;
using Alza.KafkaTest;
using Samples.KafkaConsumerStorage.Services;

namespace Samples.KafkaConsumerStorage.MessageHandlers
{
	internal class SomeAvroMessageHandler(ISomeService service) : KafkaMessageHandler<SomeAwesomeAvro>
	{
		public override async Task<IKafkaConsumptionResult> Handle(SomeAwesomeAvro message, CancellationToken cancellationToken)
		{
			if (await service.HandleAndStoreAvroData(new(message.SomeDate, message.SomeString), cancellationToken))
			{
				return KafkaConsumptionResult.Handle;
			}

			return KafkaConsumptionResult.Reject();
		}
	}
}
