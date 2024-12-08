# Samples

<details>
 <summary>Fixture</summary>

<!--codeinclude-->
[](../../tests/Samples.Worker.RabbitMq.Tests/Shared/TestFixture.cs)
<!--/codeinclude-->

</details>

<details>
 <summary>Publishing on app run in worker service</summary>

<!--codeinclude-->
[](../../samples/Samples.Worker.RabbitMq/RabbitPublishingWorker.cs)
<!--/codeinclude-->

</details>

<details>
 <summary>Test - consumming message from rabbit to assert that it was published sucessfully</summary>

<!--codeinclude-->
[](../../tests/Samples.Worker.RabbitMq.Tests/RabbitPublisherTests.cs)
<!--/codeinclude-->

</details>

<details>
 <summary>Test - publishing message to RabbitMQ which is sucessfully managed by application</summary>

<!--codeinclude-->
[](../../tests/Samples.Worker.RabbitMq.Tests/RabbitConsumerTests.cs)
<!--/codeinclude-->

</details>
