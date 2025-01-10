# MsSQL
To use MsSQL use extension method on `IApplicationFactoryBuilder` named `UseSql` with string seed and delegate containing `ConfigurationBuilder` and `SqlContainerSettings`. This will allow you to run MsSQL in docker, create tables and fill em and register it within your fixture as follows.

This extension lives in package: `Testing.Sql`

Prepare your sql seed or just use some sort of migrations in startup

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
