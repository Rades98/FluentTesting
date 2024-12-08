# Rabbit MQ

To use elasticsearch use extension method on `IApplicationFactoryBuilder` named `UseRabbitMq` 
with delegate containing `ConfigurationBuilder` and `RabbitMqContainerSettings`. There is `RabbitMQOptions` delegate aswell to define 
exchanges, routing keys, queues and bindings.
This will allow you to run RabbitMq in docker and register it within your fixture as follows.
This extension lives in package: `FluentTesting.RabbitMq`

```csharp
.UseRabbitMq((configuration, rabbitSettings) =>
            {
                configuration.AddObject("RabbitConnectionOptions", new RabbitConnectionOptions()
                {
                    HostName = rabbitSettings.Host,
                    Password = rabbitSettings.Password,
                    UserName = rabbitSettings.UserName,
                });
            }, opts =>
            {
                opts.PublisherBindings = [new Exchange() {
                    ExchangeName = "test",
                    RoutingKeys = ["testRoutingKey"]
                }];

                // Since we want to consume aswell (published message from tested app),
                // we need to add publisher bindings which will be asserter in tests
                opts.ConsumerBindings = [new Exchange() {
                    ExchangeName = "test",
                    RoutingKeys = ["testRoutingKey"]
                }];

                opts.QueueName = "testQueue";
            })
```