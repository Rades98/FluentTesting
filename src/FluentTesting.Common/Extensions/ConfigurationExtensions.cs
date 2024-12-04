namespace FluentTesting.Common.Extensions
{
    using Microsoft.Extensions.Configuration;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Configuration extensions
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add object. Since configuration is stored as bunch of env variables and not serialized JSON stuff, we need to
        /// mock configuration by this specific approach
        /// </summary>
        /// <param name="builder">configuration builder</param>
        /// <param name="sectionName">section name</param>
        /// <param name="obj">object to store</param>
        public static IConfigurationBuilder AddObject(this IConfigurationBuilder builder, string? sectionName = null, object? obj = null)
        {
            if (obj is null)
            {
                return builder;
            }

            sectionName ??= obj.GetType().Name;

            builder.AddInMemoryCollection(FlattenObject(obj, sectionName).Select(x => new KeyValuePair<string, string?>(x.Key, x.Value)));

            return builder;
        }

        /// <summary>
        /// Add key value
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddKeyValue(this IConfigurationBuilder builder, string key, string value)
        {
            builder.AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>(key, value)
            ]);

            return builder;
        }

        /// <summary>
        /// Add connection string
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="name">connection string name ex: ConnectionStrings{ &lt;name&gt;: "connection string"}</param>
        /// <param name="connectionString">connection string</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddConnectionString(this IConfigurationBuilder builder, string name, string connectionString)
        {
            builder.AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>($"ConnectionStrings:{name}", connectionString)
            ]);

            return builder;
        }

        private static Dictionary<string, string> FlattenObject(object obj, string parentKey)
        {
            var data = new Dictionary<string, string>();

            if (obj is null)
            {
                return data;
            }

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var key = $"{parentKey}:{prop.Name}";
                var value = prop.GetValue(obj);

                if (value != null)
                {
                    if (IsSimpleType(prop.PropertyType))
                    {
                        data[key] = value.ToString()!;
                    }
                    else if (value is IDictionary dictionary)
                    {
                        FlattenDictionary(dictionary, key, data);
                    }
                    else if (value is IEnumerable enumerable && value is not string)
                    {
                        var index = 0;
                        foreach (var item in enumerable)
                        {
                            var itemKey = $"{key}:{index++}";

                            if (item is null)
                            {
                                continue;
                            }

                            if (IsSimpleType(item.GetType()))
                            {
                                data[itemKey] = item.ToString()!;
                            }
                            else
                            {
                                FlattenObject(item, itemKey)
                                    .ToList()
                                    .ForEach(x => data[x.Key] = x.Value);
                            }
                        }
                    }
                    else
                    {
                        FlattenObject(value, key)
                            .ToList()
                            .ForEach(x => data[x.Key] = x.Value);
                    }
                }
            }

            return data;
        }

        private static void FlattenDictionary(IDictionary dictionary, string parentKey, IDictionary<string, string> data)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var itemKey = $"{parentKey}:{entry.Key}";
                var itemValue = entry.Value;

                if (itemValue != null)
                {
                    if (IsSimpleType(itemValue.GetType()))
                    {
                        data[itemKey] = itemValue.ToString()!;
                    }
                    else
                    {
                        FlattenObject(itemValue, itemKey)
                            .ToList()
                            .ForEach(x => data[x.Key] = x.Value);
                    }
                }
            }
        }

        private static bool IsSimpleType(Type type)
            => type.IsPrimitive 
               || type.IsEnum 
               || type == typeof(string) 
               || type == typeof(decimal) 
               || type == typeof(DateTime) 
               || type == typeof(Uri) 
               || type == typeof(TimeSpan);
    }

}
