namespace OrganisationRegistry.Configuration.Database.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;
using Infrastructure.Configuration;

public class ConfigurationDatabaseConfiguration
{
    public static string Section = "Configuration";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public string ConnectionString { get; set; } = null!;

    public ConfigurationDatabaseConfiguration Obfuscate()
    {
        return new ConfigurationDatabaseConfiguration
        {
            ConnectionString = Obfuscator.ObfuscateConnectionString(ConnectionString),
        };
    }
}
