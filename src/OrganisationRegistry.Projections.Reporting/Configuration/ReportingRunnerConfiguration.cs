namespace OrganisationRegistry.Projections.Reporting.Configuration
{
    using Newtonsoft.Json;
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;

    public class ReportingRunnerConfiguration
    {
        public static string Section = "ReportingRunner";

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
