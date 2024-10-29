using Avro.Specific;
using System.Reflection;

namespace Testing.Kafka.Serdes
{
	/// <summary>
	/// Reflection helper
	/// </summary>
	internal static class ReflectionHelper
	{
		/// <summary>
		/// Get specific types
		/// </summary>
		/// <param name="assemblies">assemblies</param>
		/// <returns>enumerable of types</returns>
		internal static IEnumerable<Type> GetSpecificTypes(IEnumerable<Assembly> assemblies)
			=> assemblies
				.SelectMany(assembly =>
				{
					if (!assembly.IsDynamic)
					{
						IEnumerable<Type> exportedTypes = null;
						try
						{
							exportedTypes = assembly.GetTypes();
						}
						catch (ReflectionTypeLoadException e)
						{
							exportedTypes = e.Types.Where(type => type is not null).ToArray()!;
						}

						return exportedTypes.Where(IsSpecificType);
					}

					return Enumerable.Empty<Type>();
				});

		/// <summary>
		/// Is specific type
		/// </summary>
		/// <param name="type">Type</param>
		/// <returns>true whether is specific</returns>
		internal static bool IsSpecificType(Type type)
			=> type.IsClass &&
				!type.IsAbstract &&
				typeof(ISpecificRecord).IsAssignableFrom(type) &&
				GetSchema(type) is not null;

		/// <summary>
		/// Get schema
		/// </summary>
		/// <param name="type">type</param>
		/// <returns>schema</returns>
		internal static Avro.Schema GetSchema(IReflect type)
			=> (Avro.Schema)type.GetField("_SCHEMA", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)!;
	}
}
