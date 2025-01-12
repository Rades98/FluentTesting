namespace Samples.AspApp.Adapters
{
	internal class BlobAdapter(AppBlobServiceClient client) : IBlobAdapter
	{
		public async Task<byte[]?> GetFileAsync(string containerName, string blobName, CancellationToken cancellationToken)
		{
			var containerClient = client.GetBlobContainerClient(containerName);

			if (!await containerClient.ExistsAsync(cancellationToken))
			{
				return null;
			}

			var blobClient = containerClient.GetBlobClient(blobName);

			if (!await blobClient.ExistsAsync(cancellationToken))
			{
				return null;
			}

			var target = new MemoryStream();
			await blobClient.DownloadToAsync(target, cancellationToken);

			return target.ToArray();
		}
	}
}
