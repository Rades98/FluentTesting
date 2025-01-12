using FluentTesting.Azurite.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentTesting.Azurite
{
	internal class BlobDataConverter : JsonConverter<BlobData>
	{
		public override BlobData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using JsonDocument doc = JsonDocument.ParseValue(ref reader);
			var root = doc.RootElement;

			var blobData = new BlobData
			{
				Name = root.GetProperty("name").GetString()!,
				ContentLength = root.GetProperty("properties").GetProperty("contentLength").GetInt64(),
				ContentType = root.GetProperty("properties").GetProperty("contentSettings").GetProperty("contentType").GetString()!,
				ContentMd5 = root.GetProperty("properties").GetProperty("contentSettings").GetProperty("contentMd5").GetString()!,
				Created = root.GetProperty("properties").GetProperty("creationTime").GetDateTimeOffset(),
				LastModified = root.GetProperty("properties").GetProperty("lastModified").GetDateTimeOffset()
			};

			return blobData;
		}

		public override void Write(Utf8JsonWriter writer, BlobData value, JsonSerializerOptions options)
			=> throw new NotImplementedException();
	}
}
