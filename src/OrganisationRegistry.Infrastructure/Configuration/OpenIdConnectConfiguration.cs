namespace OrganisationRegistry.Infrastructure.Configuration
{
    public class OpenIdConnectConfiguration
    {
        public static string Section = "OIDCAuth";

        public string Authority { get; set; }

        public string AuthorizationRedirectUri { get; set; }

        public string AuthorizationIssuer { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string UserInfoEndPoint { get; set; }

        public string EndSessionEndPoint { get; set; }

        public string JwksUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TokenEndPoint { get; set; }

        public string JwtSharedSigningKey { get; set; }

        public string JwtIssuer { get; set; }

        public string JwtAudience { get; set; }

        public string Developers { get; set; }
    }
}
