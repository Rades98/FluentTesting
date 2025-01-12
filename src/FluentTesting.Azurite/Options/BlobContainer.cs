namespace FluentTesting.Azurite.Options
{
	public class BlobContainer
	{
		public required string Name { get; set; }

		public IEnumerable<BlobFile> Files { get; set; } = [];
	}
}
