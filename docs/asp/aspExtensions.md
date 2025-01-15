# Extensions
This API comes with several extensions like JWT mocking, json assertions etc

## Mock JWT Auth
To mock jwt authentication you can simly use `RegisterAuth()` extensions on `IServiceCollection`

```csharp
ApplicationFactory = new AspApplicationFactoryBuilder<Program>()
	.RegisterServices((services, configuration) =>
		{
			services.RegisterAuth();
		})
	.Build();
```

## Send request with mocked JWT
If you have registered mocked JWT you can use several extensions simulationg user request
```csharp
var res = await fixture.Client.GetAsUserAsync("auth", 1);
```

if you want to test with specific claims and roles, you will have to use `JwtHelper` object with `GetJwt` 
method supporting definition of roles ets

```csharp
await fixture.Client
	.AddBearerAuthHeader(JwtHelper.GetJwt(userId, claims, roles))
	.GetAsync(requestUri);
```

## JSON assertation extension against file
You can assert http message against json file


```csharp
[Fact]
public async Task AnonymousEndpoint_Should_ReturnOk()
{
	var res = await fixture.Client.GetAsync("");

	res.AssertStatusCode(System.Net.HttpStatusCode.OK);

	await fixture.AssertJsonResponseAsync(res, "AssertJson.json");
}
```
!!! warning
	NOTE THAT you have to set Copy always/preserve newest on assertation file so it will appear in build folder


<details>
    <summary> Optionally you can specify assertion regex for getting json path if needed, but by default it should work via calling assembly</summary>

```csharp
ApplicationFactory = new AspApplicationFactoryBuilder<Program>()
	.SetAssertionRegex(@".*Samples\.AspApp\.Tests[\\\/]+(.*?)[\\\/](?:(?![\\\/]).)*$")
	...
	.Build();
```
</details>
