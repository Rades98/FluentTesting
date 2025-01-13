
# Redis
Registering of Redis we call `UseRedis` method on `IApplicationFactoryBuilder`, then `Action<ConfigurationBuilder, RedisContainerSettings>` to retrieve container's setting and optionally `Action<RedisOptions>` to set custom port.

Default port for accessing Redis in this container is: 6001.

```csharp
public TestsFixture()
{
	ApplicationFactory = new ApplicationFactoryBuilder<Program>()
	...
	.UseRedis(
        (configuration, containerSettings) =>
            {
                // your setting with usage of containerSettings
            }
        )
	.Build();
}
```
<details>
    <summary>(Click to expand) - Optional: custom port </summary>

```csharp
.UseRedis(
    (configuration, containerSettings) =>
        {
            // omitted for the sake of brevity
        },
    // here you can set your own options, usually can be omitted when defaults are sufficient
    options =>
        {
            options.Port = 6969;
        }
    )
```
</details>

## Seed
To register with seed fill `Seed` prop of type `Dictionary<string, string>` in options delegate
```csharp
.UseRedis(
	(configuration, settings) =>
	{
		configuration.AddConnectionString("RedisConnectionString", $"{settings.Url}:{settings.Port}");
	},
	opts =>
	{
		opts.Seed = new()
		{
			{ "someKey", "some value :)" }
		};
	})
```
