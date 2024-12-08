using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace FluentTesting.RabbitMq.Extensions
{
    internal class PrivateContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Create JsonProperty for given MemberInfo.
        /// Overriden version treats non-public properties' setters as writable.
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (prop.Writable)
                return prop;

            if (member is PropertyInfo propInfo)
            {
                var hasPrivateSetter = propInfo.GetSetMethod(nonPublic: true) != null;
                prop.Writable = hasPrivateSetter;
            }

            return prop;
        }
    }
}
