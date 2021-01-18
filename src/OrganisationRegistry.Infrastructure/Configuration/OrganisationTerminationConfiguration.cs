namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class OrganisationTerminationConfiguration
    {
        public static string Section = "OrganisationTermination";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; set; }
        public string OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; set; }
    }
}
