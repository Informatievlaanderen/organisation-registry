namespace OrganisationRegistry.Infrastructure.Configuration
{
    public class OidcClientConfiguration
    {
        public string Authority { get; }

        public string Issuer { get; }

        public string AuthorizationEndpoint { get; }

        public string UserInfoEndPoint { get; }

        public string EndSessionEndPoint { get; }

        public string JwksUri { get; }

        public string ClientId { get; }

        public string RedirectUri { get; }

        public string PostLogoutRedirectUri { get; }

        public OidcClientConfiguration(OpenIdConnectConfigurationSection configuration)
        {
            Authority = configuration.Authority;
            Issuer = configuration.AuthorizationIssuer;
            AuthorizationEndpoint = configuration.AuthorizationEndpoint;
            UserInfoEndPoint = configuration.UserInfoEndPoint;
            EndSessionEndPoint = configuration.EndSessionEndPoint;
            JwksUri = configuration.JwksUri;
            ClientId = configuration.ClientId;
            RedirectUri = configuration.AuthorizationRedirectUri;
            PostLogoutRedirectUri = configuration.PostLogoutRedirectUri;
        }
    }
}
