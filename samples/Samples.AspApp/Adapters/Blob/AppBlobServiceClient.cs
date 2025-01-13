using Azure.Storage.Blobs;

namespace Samples.AspApp.Adapters.Blob
{
	public class AppBlobServiceClient(IConfiguration configuration) : BlobServiceClient(configuration.GetConnectionString("BlobStorageConnection"))
	{
	}
}
