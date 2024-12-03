# Azure Blob
To use azure blob storage use extension method on `IApplicationFactoryBuilder` named `UseBlob` 
with delegate containing `ConfigurationBuilder` and `AzuriteContainerSettings`. 
This will allow you to run elasticsearch in docker and register it within your fixture as follows.
This extension lives in package: `FluentTesting.Azurite`

```csharp
.UseBlob((configuration, settings) =>
{
    configuration.AddConnectionString("BlobStorageConnection", settings.ConnectionString);
})
```