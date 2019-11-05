namespace OrganisationRegistry.Api.Configuration
{
    using System;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;

    public class AuthConfiguration
    {
        public static string Section = "Auth";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string JwtCookieName { get; set; }
        public string JwtSharedSigningKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
    }
}
