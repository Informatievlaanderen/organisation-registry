namespace OrganisationRegistry.Infrastructure.Configuration;

using System;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public class InfrastructureConfigurationSection
{
    public static string Name = "Infrastructure";

    [JsonConverter(typeof(TimestampConverter))]
    public DateTime Created => DateTime.Now;

    public string EventStoreConnectionString { get; set; } = null!;
    public string EventStoreAdministrationConnectionString { get; set; } = null!;
    public int EventStoreCommandTimeout { get; set; }

    public string ExternalIpServiceUri { get; set; } = null!;

    public InfrastructureConfigurationSection Obfuscate()
        => new()
        {
            EventStoreConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreConnectionString),
            EventStoreAdministrationConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreAdministrationConnectionString),
            EventStoreCommandTimeout = EventStoreCommandTimeout
        };
}