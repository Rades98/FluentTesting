using Azure.Storage.Blobs;

namespace Samples.AspApp.Adapters
{
	public class AppBlobServiceClient(IConfiguration configuration) : BlobServiceClient(configuration.GetConnectionString("BlobStorageConnection"))
	{
	}
}
