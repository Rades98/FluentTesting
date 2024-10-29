# Mocking
When dealing with asynchronous messaging, it can be challenging to determine precisely when a 
message will be consumed. This is where the use of `CancellationTokenSource` becomes essential. 
To manage these waiting operations, you have two options. The first is to use the `UnmockAndWait` extension, 
which allows you to wait for a mocked method's completion. However, this approach has some limitations: 
the mocked method can have a maximum of six arguments, and the return type must be either `Task` or `Task<TResult>`. 
The design is intentionally userfriendly.
```csharp
fixture.UnmockAndWait<IAvroHandlingService, bool, IncomingKafkaMessage, CancellationToken>(
    fixture.ServiceMock,
    src => src.EnrichAndPublishChangeEvent(It.IsAny<IncomingKafkaMessage>(), 
    It.IsAny<CancellationToken>()),
    cts);
```

In this example:

The first generic parameter is the type of the service, repository, or other managing object.
The second generic parameter is the return type of the method if it returns `Task<TResult>`. If the method returns `Task`, this parameter is omitted.
The first argument is the mocked object.
The second argument is a delegate for `Moq's` Returns method.
The third argument is a `CancellationTokenSource`, used to cancel the wait for message consumption.
The fourth argument, which is optional, is a `TimeSpan` for an additional delay, what is useful when working with databases.
The remaining generic parameters specify the types of the arguments in the mocked method.

Note: Avoid mocking `MediatR` calls unless you are familiar with its workings.

## Advanced Usage
If the `UnmockAndWait` extensions do not meet your needs, you can use a more flexible mechanism that provides greater control. 
In this case, manually set up your mock and use the `UseBaseImplementationAndCancelToken` extension. This method takes a generic 
parameter of the service, repository, or other managing object. The first argument is a delegate that performs the action on the object, 
the second is the `CancellationTokenSource`, and the third is an optional `TimeSpan` to define an additional delay.

```csharp
fixture.SampleMongoRepoMock.Setup(s => s.InsertSampleModel(It.IsAny<SampleMongoModel>(), It.IsAny<CancellationToken>()))
    .Returns((SampleMongoModel model, CancellationToken ct) =>
    {
        return fixture.UseBaseImplementationAndCancelToken<ISampleMongoRepository>(
            src => src.InsertSampleModel(model, ct),
            cts,
            TimeSpan.FromSeconds(2));
    });
```