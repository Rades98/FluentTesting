using FluentTesting.Asp.Extensions;
using FluentTesting.Azurite.Extensions;
using Samples.AspApp.Tests.Shared;

namespace Samples.AspApp.Tests.Blob
{
	[Collection("AspTestFixture")]
	public class BlobTests(TestFixture fixture)
	{
		[Fact]
		public async Task BlobEndpoint_ShouldReturnOK()
		{
			var res = await fixture.Client.GetAsync("file");

			res.AssertStatusCode(System.Net.HttpStatusCode.OK);

			await fixture.AssertFileResponseAgainstBlobMd5Async(res, "photos", "asd.png");

			//using var fileStream = await res.Content.ReadAsStreamAsync(); // Obtain specific part from multipart if needed

			//await fixture.AssertFileResponseAgainstBlobMd5Async(fileStream, "photos", "asd.png");
		}
	}
}
