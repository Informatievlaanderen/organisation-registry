namespace OrganisationRegistry.Infrastructure.Configuration;

public class OpenIdConnectConfigurationSection
{
    public static string Name = "OIDCAuth";

    public string Authority { get; set; } = null!;

    /// <summary>
    /// Optional override for server-side calls (e.g. token exchange in k8s where the public
    /// authority hostname is not resolvable from within the cluster). Falls back to Authority.
    /// </summary>
    public string? InternalAuthorityOverride { get; set; }

    public string EffectiveAuthority => InternalAuthorityOverride ?? Authority;

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

    /// <summary>
    /// Introspection endpoint for Keycloak tokens (used by BffApi scheme).
    /// Derived from EffectiveAuthority when not explicitly configured.
    /// </summary>
    public string? IntrospectionEndpoint { get; set; }

    public string EffectiveIntrospectionEndpoint =>
        IntrospectionEndpoint ?? $"{EffectiveAuthority.TrimEnd('/')}/protocol/openid-connect/token/introspect";
}
