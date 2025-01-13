namespace Samples.AspApp.Adapters.Blob
{
	public interface IBlobAdapter
	{
		Task<byte[]?> GetFileAsync(string containerName, string blobName, CancellationToken cancellationToken);
	}
}
