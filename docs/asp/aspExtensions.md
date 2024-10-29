# Extensions
This API comes with several extensions like JWT mocking, json assertions etc

## Mock JWT Auth
To mock jwt authentication you can simly use `RegisterAuth()` extensions on `IServiceCollection`

<details>
    <summary>See example </summary>

```csharp
ApplicationFactory = new AspApplicationFactoryBuilder<Program>()
	.RegisterServices((services, configuration) =>
		{
			services.RegisterAuth();
		})
	.Build();
```
</details>

## Send request with mocked JWT
If you have registered mocked JWT you can use several extensions simulationg user request
```csharp
var res = await fixture.Client.GetAsUserAsync("auth", 1);
```

if you want to test with specific claims and roles, you will have to use `JwtHelper` object with `GetJwt` 
method supporting definition of roles ets
<details>
	<summary>Usage with specific roles or claims</summary>

```csharp
client
	.AddBearerAuthHeader(JwtHelper.GetJwt(userId))
	.GetAsync(requestUri);
```
</details>

## JSON assertation extension against file
You can assert http message against json file if you will follow 2 steps

<details>
    <summary>1. You have to register your path regex to base tests directory as follows</summary>

```csharp
ApplicationFactory = new AspApplicationFactoryBuilder<Program>()
	.SetAssertionRegex(@".*Samples\.AspApp\.Tests[\\\/]+(.*?)[\\\/](?:(?![\\\/]).)*$")
	...
	.Build();
```
</details>


<details>
    <summary>2. Use it in file properly</summary>

```csharp
[Fact]
public async Task AnonymousEndpoint_Should_ReturnOk()
{
	var res = await fixture.Client.GetAsync("");

	res.AssertStatusCode(System.Net.HttpStatusCode.OK);

	await fixture.AssertJsonResponseAsync(res, "AssertJson.json");
}
```
NOTE THAT you have to set Copy always on assertation file so it will appear in build folder
</details>
