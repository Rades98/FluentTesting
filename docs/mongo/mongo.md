
# Mongo
Registering of MongoDb we call `UseMongoDb` method on `IApplicationFactoryBuilder`, giving seed (string of Mongo shell Javascript language), then `Action<ConfigurationBuilder, MongoContainerSettings>` to retrieve container's setting and optionally `Action<MongoDbOptions>` to set custom credentials, database and port.

Default port for accessing MongoDb in this container is: 49158. 

```csharp
public TestsFixture()
{
	ApplicationFactory = new ApplicationFactoryBuilder<Program>()
	...
	.UseMongo(
        seed,
        (configuration, containerSettings) =>
            {
                // example how to retrieve the settings from the container and use them as will
                configuration.AddObject("MongoConfigurationOptions", new MongoConfigurationOptions
                                                                        {
                                                                            Username = containerSettings.Username,
                                                                            Password = containerSettings.Password,
                                                                            DatabaseName = containerSettings.DatabaseName,
                                                                            Hosts = containerSettings.Hosts,
                                                                            Port = containerSettings.Port,
                                                                            UseTls = false
                                                                        });
            }
        )
	.Build();
}
```
<details>
    <summary>(Click to expand) - Optional: custom credentials and database name </summary>

```csharp
.UseMongo(
    seed,
    (configuration, containerSettings) =>
        {
            // omitted for the sake of brevity
        },
    // here you can set your own options, usually can be omitted when defaults are sufficient
    options =>
        {
            options.Username = "customlogin";
            options.Password = "password";
            options.DatabaseName = "TestingSamplesDatabase";
            options.Port = 40000;
        }
    )
```
</details>

## Authorization
Mongo database is created with superuser (credentials: admin // admin). For creating authorization user on specific database, you must create it using for example Seed script.

## Seed
Seed script is executed immediately after container starts and server the purpose of initializing database. Script is accepted in Mongo shell Javascript language.
Example:
```javascript
db = db.getSiblingDB("TestingSamplesDatabase");
db.createUser({
    user: "customlogin",
    pwd: "password",
    roles: [
        {
            role: "readWrite",
            db: "TestingSamplesDatabase"
        }
    ]
})
db.SampleCollection.insertOne({
    text: "abcd",
    date: new ISODate("2024-08-21T13:09:12Z")
});
```
Optionally you can use provided SeedBuilder to generate parts (or all) of the required Seed.
Using SeedBuilder to generate the script above:
```csharp
string seed = new SeedBuilder("TestingSamplesDatabase")
    .CreateUser("customlogin", "password")
    .InsertDocument("SampleCollection",
        """
            {
                text: "abcd",
                date: new ISODate("2024-08-21T13:09:12Z")
            }
        """)
    .Build();
```
## Usage in tests
Following example shows how to assert data from Mongo using `MongoDb.Driver`. Implementation may differ with other packages.
1. Create Mongo client in your Fixture `MongoClient = ApplicationFactory.Services.GetRequiredService<IMongoClient>();`
2. In tests' assertation retrieve data from the Mongo using client from fixture `var result = await fixture.MongoClient.GetDatabase(DbName).GetCollection<SampleMongoModel>(...`
3. Assert the `result` as needed