# MsSQL
To use MsSQL use extension method on `IApplicationFactoryBuilder` named `UseSql` with string seed and delegate containing `ConfigurationBuilder` and `SqlContainerSettings`. This will allow you to run MsSQL in docker, create tables and fill em and register it within your fixture as follows.

This extension lives in package: `Testing.Sql`

Prepare your sql seed

```csharp
var sqlSeed = "CREATE TABLE etc etc...";
```
Use it in registration 

```csharp 
.UseSql(sqlSeed, (configuration, options) =>
{
    configuration.AddConnectionString("Web", options.ConnectionString);
})
```
