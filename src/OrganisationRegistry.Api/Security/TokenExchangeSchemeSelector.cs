namespace OrganisationRegistry.Api.Security;

using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// ForwardDefaultSelector of the "BearerOrIntrospection" policy scheme: a JWT (3 dot-separated segments)
    /// goes to JwtBearer; an opaque token goes to EditApi on Edit API routes and to TokenExchange elsewhere.
    /// </summary>
    public static string SelectDefaultScheme(HttpContext context, bool tokenExchangeEnabled)
    {
        if (!tokenExchangeEnabled)
            return JwtBearerDefaults.AuthenticationScheme;

        var header = context.Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return JwtBearerDefaults.AuthenticationScheme;

        var token = header["Bearer ".Length..].Trim();
        if (token.Count(c => c == '.') == 2)
            return JwtBearerDefaults.AuthenticationScheme;

        // Opaque token on an Edit API route is an M2M token: introspect once, with the EditApi client
        return IsEditApiRoute(context.Request.Path)
            ? AuthenticationSchemes.EditApi
            : AuthenticationSchemes.TokenExchange;
    }

    /// <summary>True for /v{version}/edit/... routes.</summary>
    public static bool IsEditApiRoute(PathString path)
        => path.Value is { } p
           && p.Split('/', StringSplitOptions.RemoveEmptyEntries) is [_, var area, ..]
           && area.Equals("edit", StringComparison.OrdinalIgnoreCase);
}
