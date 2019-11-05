namespace OrganisationRegistry.Configuration.Database.Configuration
{
    using System;
    using Newtonsoft.Json;
    using Infrastructure.Configuration;
    using Infrastructure.Infrastructure.Json;

    public class ConfigurationDatabaseConfiguration
    {
        public static string Section = "Configuration";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string ConnectionString { get; set; }

        public ConfigurationDatabaseConfiguration Obfuscate()
        {
            return new ConfigurationDatabaseConfiguration
            {
                ConnectionString = Obfuscator.ObfuscateConnectionString(ConnectionString),
            };
        }
    }
}
