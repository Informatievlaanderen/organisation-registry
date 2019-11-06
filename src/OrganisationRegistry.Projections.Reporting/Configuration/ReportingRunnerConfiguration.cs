namespace OrganisationRegistry.Projections.Reporting.Configuration
{
    using Newtonsoft.Json;
    using System;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;

    public class ReportingRunnerConfiguration
    {
        public static string Section = "ReportingRunner";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;
    }

}
