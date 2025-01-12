namespace Samples.AspApp.Adapters
{
	public interface IBlobAdapter
	{
		Task<byte[]?> GetFileAsync(string containerName, string blobName, CancellationToken cancellationToken);
	}
}
