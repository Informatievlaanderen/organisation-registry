namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class HostedServicesConfigurationSection
    {
        public static string Name = "HostedServices";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public HostedServiceConfigurationSection? SyncFromKboService { get; set; }
        public HostedServiceConfigurationSection? ScheduledCommandsService { get; set; }
    }
}
