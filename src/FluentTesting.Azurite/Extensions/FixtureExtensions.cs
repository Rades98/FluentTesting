using FluentAssertions;
using FluentTesting.Common.Interfaces;
using System.Security.Cryptography;

namespace FluentTesting.Azurite.Extensions
{
	public static class FixtureExtensions
	{
		/// <summary>
		/// Get informations about blob
		/// </summary>
		/// <param name="fixture">fixture</param>
		/// <param name="containerName">Azure blob container name</param>
		/// <param name="fileName">Blob name</param>
		/// <returns></returns>
		public static Task<BlobData?> GetBlobInformationsAsync(this ITestFixture fixture, string containerName, string fileName)
			=> fixture.ApplicationFactory.GetBlobInformationsAsync(containerName, fileName);

		/// <summary>
		/// Validates returned file stream against blob file via MD5 hash check
		/// </summary>
		/// <param name="fixture">fixture</param>
		/// <param name="response">http response containing file stream</param>
		/// <param name="containerName">Blob container name</param>
		/// <param name="blobName">Blob name with extension</param>
		public static async Task AssertFileResponseAgainstBlobMd5Async(this ITestFixture fixture, HttpResponseMessage response, string containerName, string blobName)
		{
			using var fileStream = await response.Content.ReadAsStreamAsync();
			fileStream.Seek(0, SeekOrigin.Begin);

			var blobInfo = await fixture.GetBlobInformationsAsync(containerName, blobName);

			blobInfo.Should().NotBeNull();

			if (blobInfo is not null)
			{
				using var md5 = MD5.Create();
				var fileMd5 = md5.ComputeHash(fileStream);
				fileMd5.Should().BeEquivalentTo(Convert.FromBase64String(blobInfo.ContentMd5));
			}
		}
	}
}
