# Extensions

## Backup and Restore
On `ITestsFixture` and `IApplicationFactory` is extension `BackupMsSqlDatabaseAsync` which creates backup of whole database. To restore state,
use `RestoreMsSqlDatabasesAsync`. 
To make this whole work, ensure that you are ***not using master*** as db. This can be achieved by defining database
name in `UseSql` settings as shown bellow:

```cs
.UseSql(SqlSeed, (configuration, sqlSettings) =>
{
    configuration.AddConnectionString("Web", sqlSettings.ConnectionString);
}, opts =>
{
    opts.Database = "TestDb"; //specify name, default is master
})
```

```csharp
[Fact]
public async Task SomeUpdatingOrCreatingtestScenario()
{
	//Create backup
	await fixture.BackupMsSqlDatabasesAsync();

	var res = await fixture.Client.PutAsync("sql", someObject);

	res.AssertStatusCode(System.Net.HttpStatusCode.OK);

	//Restore data
	await fixture.RestoreMsSqlDatabasesAsync();
}

```

## Obtain data from sql for futher use
To Get data there are extension on `ITestsFixture` and `IApplicationFactory` named `GetMsSqlObjectAsync` and `GetRawMsSqlObjectAsync`

This one is trying to map data for you from sql response - for mapping purposes is used JSON, 
so you should reflect naming etc or use native .NET JSON attributes to achieve so.

```csharp
var obj = await fixture.GetMsSqlObjectAsync<SomeTable, int>("SomeTable", 1, "Id");

obj?.SomeString.Should().Be("MyString");
```

Some error may occure in serialization proces, co there is even GetRawMsSqlObjectAsync which returns raw data 

!!! warning "Raw string"
    This extension returns response from SQL server running inside container, so it is just string which needs some work, but as was foretold, some problems in serialization may occure, so to not let you without this functionality this one comes to play

```csharp
var data = await fixture.GetRawMsSqlObjectAsync("SomeTable", 1, "Id");
```

<details>
	<summary>Example of string data representation returned from container</summary>
```txt
Id          SomeInt     SomeString                    
----------- ----------- ------------------------------
			1           0 kokos                         

(1 rows affected)
```
</details>