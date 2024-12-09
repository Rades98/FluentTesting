# Extensions

## Backup and Restore
On `ITestsFixture` and `IApplicationFactory` is extension `BackupMsSqlDatabaseAsync` which creates backup of whole database. To restore state,
use `RestoreMsSqlDatabasesAsync`. To make this whole work, ensure that you are not using master as db. This can be achieved by defining database
name in `UseSql` settings as shown bellow:

```csharp
.UseSql(SqlSeed, (configuration, sqlSettings) =>
{
    configuration.AddConnectionString("Web", sqlSettings.ConnectionString);
}, opts =>
{
    opts.Database = "TestDb"; //specify name, default is master
})
```