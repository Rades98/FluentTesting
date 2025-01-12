# Azure Blob
To use azure blob storage use extension method on `IApplicationFactoryBuilder` named `UseBlob` 
with delegate containing `ConfigurationBuilder` and `AzuriteContainerSettings`. 
This will allow you to run elasticsearch in docker and register it within your fixture as follows.
This extension lives in package: `FluentTesting.Azurite`

```csharp
.UseAzurite((configuration, settings) =>
{
    configuration.AddConnectionString("BlobStorageConnection", settings.ConnectionString);
})
```

## Seed
To seed data you can add `AzuriteOptions` filling delegate where is property named BlobSeed. Seed has structure as follows 
where `Name` is name of container (in means of BLOB) and `Files` has props for `Path` and optionally for specific file name.
To set custom name set `Name` property, otherwise is used file name from `Path`

```csharp
.UseAzurite(
	(configuration, settings) =>
	{
		configuration.AddConnectionString("BlobStorageConnection", settings.ConnectionString);
	},
	opts =>
	{
		opts.BlobSeed =
		[
			new()
			{
				Name = "photos",
				Files =
				[
					new()
					{
						// Update file path to yor needs, this leads to <TestProj>/Share/asd.png file as is in Samples.AspApp.Tests
						Path = Path.Combine(Directory.GetCurrentDirectory(), "Shared", "asd.png")
					}
				]
			}
		];
	}
)
```