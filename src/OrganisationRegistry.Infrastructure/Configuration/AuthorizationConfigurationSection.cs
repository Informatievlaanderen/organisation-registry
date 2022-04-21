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

        public string FormalFrameworkIdsOwnedByVlimpers { get; set; } = null!;
        public string FormalFrameworkIdsOwnedByAuditVlaanderen { get; set; } = null!;
        public string LabelIdsAllowedForVlimpers { get; set; } = null!;
        public string KeyIdsAllowedForVlimpers { get; set; } = null!;
        public string KeyIdsAllowedOnlyForOrafin { get; set; } = null!;
        public string FormalFrameworkIdsOwnedByRegelgevingDbBeheerder { get; set; } = null!;
        public string OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder { get; set; } = null!;
        public string CapacityIdsOwnedByRegelgevingDbBeheerder { get; set; } = null!;
    }
}
