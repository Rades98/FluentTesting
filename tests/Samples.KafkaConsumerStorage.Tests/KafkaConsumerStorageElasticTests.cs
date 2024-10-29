using Alza.KafkaTest;
using FluentAssertions;
using Moq;
using Samples.KafkaConsumerStorage.Contracts;
using Samples.KafkaConsumerStorage.Data;
using Samples.KafkaConsumerStorage.Tests.Shared;
using Testing.Common.Extensions;
using Testing.Kafka;

namespace Samples.KafkaConsumerStorage.Tests
{
	[Collection("KafkaConsumerStorage")]
	public class KafkaConsumerStorageElasticTests(TestsFixture fixture)
	{
		[Fact]
		public async Task HandlerHandlesAndStoresData()
		{
			var request = new SomeAwesomeAvro()
			{
				SomeDate = DateTime.Now,
				SomeString = "Hello",
			};

			var cts = fixture.GetCtsWithTimeoutInSeconds();

			fixture.UnmockAndWait<ISampleElasticRepo, bool, IncomingKafkaMessage, CancellationToken>(
				fixture.RepoMock,
				src => src.IndexData(It.IsAny<IncomingKafkaMessage>(), It.IsAny<CancellationToken>()),
				cts, TimeSpan.FromSeconds(2)
			);

			await fixture.PublishAvroToKafkaAndWaitForConsumptionAsync(request, "incoming.topic", "someKey", null, cts);

			var searchResult = await fixture.ElasticClient.SearchAsync<InsertObject>(s => s.From(0).Query(q => q.MatchAll()));

			searchResult.Documents.Count.Should().Be(1);
		}
	}
}