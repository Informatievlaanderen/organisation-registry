namespace OrganisationRegistry.Infrastructure.Infrastructure.Json
{
    using Be.Vlaanderen.Basisregisters.Converters.TrimString;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class JsonSerializerSettingsExtensions
    {
        /// <summary>
        /// Sets up and adds additional converters for OrganisationRegistry to the JsonSerializerSettings
        /// </summary>
        /// <param name="source"></param>
        /// <returns>the updated JsonSerializerSettings</returns>
        public static JsonSerializerSettings ConfigureForOrganisationRegistry(this JsonSerializerSettings source)
        {
            var wegwijsSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();

            source.ContractResolver = wegwijsSettings.ContractResolver;

            var resolver = source.ContractResolver as DefaultContractResolver;
            if (resolver != null)
                resolver.NamingStrategy.ProcessDictionaryKeys = false;

            source.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            source.DateFormatString = "yyyy-MM-dd";
            source.Converters.Add(new TrimStringConverter());
            source.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            source.Converters.Add(new GuidConverter());

            return source;
        }
    }
}
