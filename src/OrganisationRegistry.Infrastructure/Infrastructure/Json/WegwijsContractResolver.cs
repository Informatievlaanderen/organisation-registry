namespace OrganisationRegistry.Infrastructure.Infrastructure.Json
{
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class OrganisationRegistryContractResolver : DefaultContractResolver
    {
        public bool SetStringDefaultValueToEmptyString { get; set; }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (prop.PropertyType == typeof(string) && SetStringDefaultValueToEmptyString)
                prop.DefaultValue = "";

            return prop;
        }
    }
}
