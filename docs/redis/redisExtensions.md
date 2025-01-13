
# Extensions

## GetRedisKeysAsync
To obtain all Redis keys, you can use the `GetRedisKeysAsync` extension on `ITestFixture` and `IApplicationFactory`. 
This method accepts an optional `pattern` argument to filter keys by pattern.

```csharp
var allKeys = await fixture.GetRedisKeysAsync();

allKeys.Should().NotBeEmpty();
allKeys.Length.Should().Be(5);
```

## GetRedisEntryValueAsync
To retrieve the value of a specific Redis entry for further assertions, you can use the `GetRedisEntryValueAsync` 
extension on `ITestFixture` and `IApplicationFactory`, passing the `key` of the entry you want to access.

```csharp
var val = await fixture.GetRedisEntryValueAsync("someKey");

val.Should().Be("some new value");
```

## AssertRedisValueAsync
If you don't want to manually fetch the value and assert it, you can use the `AssertRedisValueAsync` extension 
on `ITestFixture` and `IApplicationFactory`. It takes the `key` and the expected value (represented as a string) as arguments. 
If you're using some form of compression or encoding, you will need to handle that separately.

```csharp
await fixture.AssertRedisValueAsync("someKey", "string representation of desired value");
```
