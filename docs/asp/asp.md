# ASP.NET

Usage is simple, create your application Fixture, which will be used in tests and create `AspApplicationFactoryBuilder<TProgram>`object. 
This fixture should inherit from `ITestFixture`, so you will be able to use more extensions way fluently.

To add mocks use `RegisterServices(Action<IServiceCollection, IConfiguration> )` method which has as parameter delegate containing `IServiceProvider` and `IConfiguration` 

To add configuration to provider, use `UseConfiguration(Action<ConfigurationBuilder> )` method which has as parameter delegate containing `ConfigurationBuilder`
Note that all configuration providers are removed in tests, so appSettings.*.json or any sort of .env files won't work here. You have to specify all needed configuration in this method.

```csharp
public class TestFixture : ITestFixture
{
	public IApplicationFactory ApplicationFactory { get; }

	public HttpClient Client { get; }

	public TestFixture()
	{
		ApplicationFactory = new AspApplicationFactoryBuilder<Program>()
			.RegisterServices((services, configuration) =>
				{
					//register your services etc
				})
			.Build();

		Client = ApplicationFactory.GetClient();
	}
}
```
