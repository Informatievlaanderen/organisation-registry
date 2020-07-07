namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class InfrastructureConfiguration
    {
        public static string Section = "Infrastructure";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string EventStoreConnectionString { get; set; }
        public string EventStoreAdministrationConnectionString { get; set; }
        public int EventStoreCommandTimeout { get; set; }

        public string ExternalIpServiceUri { get; set; }

        public InfrastructureConfiguration Obfuscate()
            => new InfrastructureConfiguration
            {
                EventStoreConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreConnectionString),
                EventStoreAdministrationConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreAdministrationConnectionString),
                EventStoreCommandTimeout = EventStoreCommandTimeout
            };
    }
}
