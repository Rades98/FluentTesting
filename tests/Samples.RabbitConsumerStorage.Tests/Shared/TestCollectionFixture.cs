using Xunit;

namespace Samples.RabbitConsumerStorage.Tests.Shared;

[CollectionDefinition("RabbitConsumerWithMongoAndRedis")]
public class TestCollectionFixture : ICollectionFixture<TestFixture>;