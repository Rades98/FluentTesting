namespace FluentTesting.Azurite
{
	public class BlobData
	{
		/// <summary>
		/// File name
		/// </summary>
		public required string Name { get; set; }

		/// <summary>
		/// Content length
		/// </summary>
		public required long ContentLength { get; set; }

		/// <summary>
		/// Content type
		/// </summary>
		public required string ContentType { get; set; }

		/// <summary>
		/// Content MD5 hash for futher assertions
		/// </summary>
		public required string ContentMd5 { get; set; }

		/// <summary>
		/// Created
		/// </summary>
		public required DateTimeOffset Created { get; set; }

		/// <summary>
		/// Last modified
		/// </summary>
		public required DateTimeOffset LastModified { get; set; }
	}
}
