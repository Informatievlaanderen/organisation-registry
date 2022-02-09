namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class InfrastructureConfigurationSection
    {
        public static string Name = "Infrastructure";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string EventStoreConnectionString { get; set; }
        public string EventStoreAdministrationConnectionString { get; set; }
        public int EventStoreCommandTimeout { get; set; }

        public string ExternalIpServiceUri { get; set; }

        public InfrastructureConfigurationSection Obfuscate()
            => new InfrastructureConfigurationSection
            {
                EventStoreConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreConnectionString),
                EventStoreAdministrationConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreAdministrationConnectionString),
                EventStoreCommandTimeout = EventStoreCommandTimeout
            };
    }
}
