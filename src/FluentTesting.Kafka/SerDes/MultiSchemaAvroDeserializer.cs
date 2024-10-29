using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Reflection;

namespace Testing.Kafka.Serdes
{
	public class MultiSchemaAvroDeserializer : IAsyncDeserializer<ISpecificRecord>
	{
		private const byte MagicByte = 0;
		private readonly Dictionary<string, IAsyncDeserializer<ISpecificRecord>> _deserializersBySchemaName;
		private readonly ConcurrentDictionary<int, IAsyncDeserializer<ISpecificRecord>> _deserializersBySchemaId;
		private readonly ISchemaRegistryClient _schemaRegistryClient;
		private readonly bool _allowNulls;

		public MultiSchemaAvroDeserializer(ISchemaRegistryClient schemaRegistryClient, AvroDeserializerConfig avroDeserializerConfig = null, bool allowNulls = false)
			: this(schemaRegistryClient,
				   ReflectionHelper.GetSpecificTypes(AppDomain.CurrentDomain.GetAssemblies()),
				   avroDeserializerConfig,
				   allowNulls,
				   checkTypes: false)
		{
		}

		private MultiSchemaAvroDeserializer(ISchemaRegistryClient schemaRegistryClient, IEnumerable<Type> types, AvroDeserializerConfig avroDeserializerConfig, bool allowNulls, bool checkTypes = true)
		{
			_schemaRegistryClient = schemaRegistryClient ?? throw new ArgumentNullException(nameof(schemaRegistryClient));
			_allowNulls = allowNulls;

			var typeArray = (types ?? throw new ArgumentNullException(nameof(types))).ToArray();

			if (typeArray.Length == 0)
			{
				throw new ArgumentException("Type collection must contain at least one item.", nameof(types));
			}

			if (checkTypes && !typeArray.All(ReflectionHelper.IsSpecificType))
			{
				throw new ArgumentOutOfRangeException(nameof(types));
			}

			var dictTypeArray = typeArray.DistinctBy(x => GetSchema(x).Fullname);

			_deserializersBySchemaId = new ConcurrentDictionary<int, IAsyncDeserializer<ISpecificRecord>>();
			_deserializersBySchemaName = dictTypeArray.ToDictionary(t => GetSchema(t).Fullname, t => CreateDeserializer(t, avroDeserializerConfig));
		}

		public async Task<ISpecificRecord> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
		{
			if (isNull && _allowNulls)
			{
				return null;
			}

			var deserializer = await GetDeserializer(data).ConfigureAwait(continueOnCapturedContext: false);

			return deserializer == null ? null : await deserializer.DeserializeAsync(data, isNull, context).ConfigureAwait(continueOnCapturedContext: false);
		}

		private IAsyncDeserializer<ISpecificRecord> CreateDeserializer(Type specificType, AvroDeserializerConfig avroDeserializerConfig)
			=> (IAsyncDeserializer<ISpecificRecord>)Activator
					.CreateInstance(typeof(DeserializerWrapper<>).MakeGenericType(specificType), _schemaRegistryClient, avroDeserializerConfig)!;

		private async Task<IAsyncDeserializer<ISpecificRecord>> GetDeserializer(ReadOnlyMemory<byte> data)
		{
			var schemaId = GetSchemaId(data.Span);

			if (_deserializersBySchemaId.TryGetValue(schemaId, out var deserializer))
			{
				return deserializer;
			}

			var confluentSchema = await _schemaRegistryClient.GetSchemaAsync(schemaId).ConfigureAwait(continueOnCapturedContext: false);
			var avroSchema = Avro.Schema.Parse(confluentSchema.SchemaString);

			_ = _deserializersBySchemaName.TryGetValue(avroSchema.Fullname, out deserializer);
			_ = _deserializersBySchemaId.TryAdd(schemaId, deserializer);

			return deserializer;
		}

		private static Avro.Schema GetSchema(IReflect type)
			=> (Avro.Schema)type.GetField("_SCHEMA", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)!;

		private static int GetSchemaId(ReadOnlySpan<byte> data)
		{
			if (data.Length < 5)
			{
				throw new InvalidDataException($"Expecting data framing of length 5 bytes or more but total data size is {data.Length} bytes");
			}

			if (data[0] is not MagicByte)
			{
				throw new InvalidDataException($"Expecting data with Confluent Schema Registry framing. Magic byte was {data[0]}, expecting {MagicByte}");
			}

			var schemaId = BinaryPrimitives.ReadInt32BigEndian(data.Slice(1));

			return schemaId;
		}

		private class DeserializerWrapper<T>(ISchemaRegistryClient schemaRegistryClient, AvroDeserializerConfig? avroDeserializerConfig = null)
			: IAsyncDeserializer<ISpecificRecord> where T : ISpecificRecord
		{
			private readonly AvroDeserializer<T> _avroDeserializer = new(schemaRegistryClient ?? throw new ArgumentNullException(nameof(schemaRegistryClient)), avroDeserializerConfig);

			public async Task<ISpecificRecord> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
				=> await _avroDeserializer.DeserializeAsync(data, isNull, context).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
