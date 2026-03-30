namespace OrganisationRegistry.Api.Infrastructure.Security;

public static class AuthenticationSchemes
{
    /// <summary>M2M client credentials (EditApi / orafinClient / cjmClient)</summary>
    public const string EditApi = "Introspection";

    /// <summary>Legacy alias — same scheme as EditApi</summary>
    public const string Introspection = EditApi;

    /// <summary>Nuxt BFF — OAuth2 introspection of Keycloak tokens</summary>
    public const string BffApi = "BffApi";

    public const string LegacySelfMintedToken = "LegacySelfMintedToken";
}
