namespace OrganisationRegistry.ElasticSearch.Projections.Metrics
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class AppMetricsConfiguration
    {
        public const string Section = "Configuration";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        /// <summary>
        /// How much time between AppMetrics Flushes (in seconds)
        /// </summary>
        public int FlushInterval { get; set; } = 60;
    }
}
