namespace OrganisationRegistry.Infrastructure.Infrastructure.Json
{
    using System;
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

            var maybeResolver = (DefaultContractResolver?)source.ContractResolver;
            if (maybeResolver is not { } resolver)
                throw new NullReferenceException("Resolver should not be null");

            if (resolver.NamingStrategy is not { } namingStrategy)
                throw new NullReferenceException("Resolver.NamingStrategy should not be null");

            namingStrategy.ProcessDictionaryKeys = false;

            source.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            source.DateFormatString = "yyyy-MM-dd";
            source.Converters.Add(new TrimStringConverter());
            source.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
            source.Converters.Add(new GuidConverter());

            return source;
        }

        public static JsonSerializerSettings ConfigureForOrganisationRegistryEventStore(this JsonSerializerSettings source)
        {
            var wegwijsSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();

            source.ContractResolver = wegwijsSettings.ContractResolver;

            var maybeResolver = (DefaultContractResolver?)source.ContractResolver;
            if (maybeResolver is not { } resolver)
                throw new NullReferenceException("Resolver should not be null");

            if (resolver.NamingStrategy is not { } namingStrategy)
                throw new NullReferenceException("Resolver.NamingStrategy should not be null");

            namingStrategy.ProcessDictionaryKeys = false;

            source.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            source.DateFormatString = "yyyy-MM-dd";
            source.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
            source.Converters.Add(new GuidConverter());

            return source;
        }
    }
}
