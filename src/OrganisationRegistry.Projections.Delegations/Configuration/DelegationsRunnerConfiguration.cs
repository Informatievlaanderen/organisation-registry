namespace OrganisationRegistry.Projections.Delegations.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class DelegationsRunnerConfiguration
    {
        public static string Section = "DelegationsRunner";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string LockRegionEndPoint { get; set; } = null!;
        public string LockAccessKeyId { get; set; } = null!;
        public string LockAccessKeySecret { get; set; } = null!;
        public string LockTableName { get; set; } = null!;
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }
    }
}
