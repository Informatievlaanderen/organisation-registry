namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class AuthorizationConfigurationSection
    {
        public static string Name = "Authorization";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string FormalFrameworkIdsOwnedByVlimpers { get; set; }
        public string FormalFrameworkIdsOwnedByAuditVlaanderen { get; set; }
    }
}
