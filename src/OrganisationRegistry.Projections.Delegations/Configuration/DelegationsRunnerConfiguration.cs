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
    }
}
