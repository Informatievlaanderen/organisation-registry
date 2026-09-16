namespace OrganisationRegistry.Api.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrganisationRegistry.Api.Infrastructure.Security;

/// <summary>
/// Decides whether the "TokenExchange" authentication scheme should be attempted, based on the
/// "TokenExchange" configuration section. This exists to prevent an unhandled
/// <see cref="System.InvalidOperationException"/> (thrown by OAuth2IntrospectionOptions.Validate()) when
/// the "TokenExchange" section is absent or disabled (e.g. not yet rolled out to an environment) but a
/// policy still forces authentication against that scheme.
/// </summary>
public static class TokenExchangeSchemeSelector
{
    public static bool IsEnabled(TokenExchangeConfiguration? configuration)
        => configuration is not null && configuration.Enabled;

    /// <summary>
    /// Authentication schemes to register on policies or attempt in
    /// <see cref="OrganisationRegistry.Api.Infrastructure.Security.ConfigureClaimsPrincipalSelectorMiddleware"/>.
    /// Always starts from <paramref name="baseSchemes"/> and appends "TokenExchange" only when it is
    /// actually configured and enabled.
    /// </summary>
    public static string[] WithTokenExchangeIfEnabled(string[] baseSchemes, bool tokenExchangeEnabled)
    {
        if (!tokenExchangeEnabled)
            return baseSchemes;

        var schemes = new string[baseSchemes.Length + 1];
        System.Array.Copy(baseSchemes, schemes, baseSchemes.Length);
        schemes[^1] = AuthenticationSchemes.TokenExchange;
        return schemes;
    }
}
