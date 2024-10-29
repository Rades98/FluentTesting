using Nest;
using Samples.KafkaConsumerStorage.Contracts;

namespace Samples.KafkaConsumerStorage.Data
{
	internal class SampleElasticRepo(IElasticClient client) : ISampleElasticRepo
	{
		public async Task<bool> IndexData(IncomingKafkaMessage message, CancellationToken cancellationToken)
		{
			var result = await client.IndexAsync(new InsertObject(message.DateTime, message.String), i => i, cancellationToken);

			return result.IsValid;
		}
	}
}
