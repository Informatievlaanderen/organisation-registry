namespace OrganisationRegistry.Projections.Delegations.Configuration
{
    using System;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;

    public class DelegationsRunnerConfiguration
    {
        public static string Section = "DelegationsRunner";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string LockRegionEndPoint { get; set; }
        public string LockAccessKeyId { get; set; }
        public string LockAccessKeySecret { get; set; }
        public string LockTableName { get; set; }
        public int LockLeasePeriodInMinutes { get; set; }
        public bool LockEnabled { get; set; }
    }
}
