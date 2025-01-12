# Common informations

## Docker proxy
Since test runs can consume the maximum number of free pools available in the Docker registry, 
there is an extension on `IApplicationFactory` where you can set the proxy path for pulling containers.

```csharp

// Works for ASP aswell
ApplicationFactory = new ApplicationFactoryBuilder<Program>()
	...
	.UseProxiedImages("<YourDockerRegistryProxy>")
	...
	.Build();
```
___