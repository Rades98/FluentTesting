# Extensions

## Get informations
To obtain some sort of informations such as file name, content length, content type, md5, created and last modified, 
you can use `GetBlobInformationsAsync` extension on `ITestFixture` and `IApplicationFactory` where arguments are 
container name and blob file name with extension.

<details>
	<summary>Blob infos model</summary>
	
<!--codeinclude-->
[](../../src/FluentTesting.Azurite/BlobData.cs)
<!--/codeinclude-->

</details>


```csharp
var blobInfos = await fixture.GetBlobInformationsAsync("photos", "asd.png");

blobInfos.LastModified.Should().BeLessThan(TimeSpan.FromTicks(DateTimeOffset.Now.Ticks));
```

!!! note
	To assert MD5 hash you can use predefined Extension	`AssertFileResponseAgainstBlobMd5Async`

## Assert MD5
`AssertFileResponseAgainstBlobMd5Async` extension on `ITestFixture` validates `HttpResponseMessage` representing `FileContentResponse` or any other version of stream against blob file via MD5 hash.
Arguments are response, container name and file name with extension.

```csharp
await fixture.AssertFileResponseAgainstBlobMd5Async(res, "photos", "asd.png");
```

!!! warning
	If returns that blobInfo is null, you should double check that container name and file are filled correctly!