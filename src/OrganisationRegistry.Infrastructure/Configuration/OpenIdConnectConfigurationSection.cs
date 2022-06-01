namespace OrganisationRegistry.Infrastructure.Configuration;

public class OpenIdConnectConfigurationSection
{
    public static string Name = "OIDCAuth";

    public string Authority { get; set; } = null!;

    public string AuthorizationRedirectUri { get; set; } = null!;

    public string AuthorizationIssuer { get; set; } = null!;

    public string AuthorizationEndpoint { get; set; } = null!;

    public string UserInfoEndPoint { get; set; } = null!;

    public string EndSessionEndPoint { get; set; } = null!;

    public string JwksUri { get; set; } = null!;

    public string PostLogoutRedirectUri { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string TokenEndPoint { get; set; } = null!;

    public string JwtSharedSigningKey { get; set; } = null!;

    public string JwtIssuer { get; set; } = null!;

    public string JwtAudience { get; set; } = null!;

    public string? Developers { get; set; }

    public int JwtExpiresInMinutes { get; set; }
}
