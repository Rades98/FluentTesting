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

## Port Setting
Each `<Technology>Options` contains a `Port` property, which allows you to set the desired port for running the container.
Example for setting the port for SQL:

```csharp
.UseSql(SqlSeed, (configuration, sqlSettings) =>
{
	// SQL configuration, e.g., connection or database settings
}, opts =>
{
	// Set the port where the SQL container will run
	opts.Port = 1200;
})
```

!!! note "Technology"
	In this case, `<Technology>` is a placeholder for any specific technology (like `Sql`, `Redis`, etc.), depending on your context.
