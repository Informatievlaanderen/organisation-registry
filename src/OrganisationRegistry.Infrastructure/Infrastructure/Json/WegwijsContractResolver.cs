namespace OrganisationRegistry.Infrastructure.Infrastructure.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class OrganisationRegistryContractResolver : DefaultContractResolver
    {
        public bool SetStringDefaultValueToEmptyString { get; set; }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType is not { } propertyType)
                return property;
            if (property.PropertyName is not { } propertyName)
                return property;

            property = HandleDefaultString(property, propertyType, SetStringDefaultValueToEmptyString);

            property = HandleEmptyArray(property, propertyType, propertyName);

            return property;
        }

        /// <summary>
        /// Empty arrays are removed from the JSON
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static JsonProperty HandleEmptyArray(JsonProperty property, Type propertyType, string propertyName)
        {
            if (propertyType == typeof(string))
                return property;

            if (propertyType.GetInterface(nameof(IEnumerable)) == null)
                return property;

            property.ShouldSerialize =
                instance =>
                {
                    var propertyInfo = instance.GetType().GetProperties()
                        .Single(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

                    if (propertyInfo.GetValue(instance) as IEnumerable<object> is { } enumerable)
                        return enumerable.Any();

                    return false;
                };

            return property;
        }

        private static JsonProperty HandleDefaultString(JsonProperty property, Type propertyType, bool setStringDefaultValueToEmptyString)
        {
            if (propertyType == typeof(string) && setStringDefaultValueToEmptyString)
                property.DefaultValue = "";

            return property;
        }
    }
}
