namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure.Authorization;

public class ConfigureClaimsPrincipalSelectorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly bool _tokenExchangeEnabled;

    public ConfigureClaimsPrincipalSelectorMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        var tokenExchangeConfiguration = configuration.GetSection("TokenExchange").Get<TokenExchangeConfiguration>();
        _tokenExchangeEnabled = TokenExchangeSchemeSelector.IsEnabled(tokenExchangeConfiguration);
    }

    public Task Invoke(HttpContext context, IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal.ClaimsPrincipalSelector = () =>
        {
            try
            {
                if (TryGetAuthInfo(httpContextAccessor) is not { Principal: { } principal }) return null!;

                if (principal.Identity is not ClaimsIdentity user) return principal;

                var ip = context.Request.HttpContext.Connection.RemoteIpAddress;

                if (!user.HasClaim(x => x.Type == AcmIdmConstants.Claims.Ip))
                    user.AddClaim(new Claim(AcmIdmConstants.Claims.Ip, ip?.ToString() ?? "Unknown", ClaimValueTypes.String));

                return principal;
            }
            catch
            {
                return null!;
            }
        };

        return _next(context);
    }

    private AuthenticateResult? TryGetAuthInfo(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        var schemes = TokenExchangeSchemeSelector.WithTokenExchangeIfEnabled(
            new[] { JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes.EditApi },
            _tokenExchangeEnabled);

        foreach (var scheme in schemes)
        {
            var result = httpContext.GetAuthenticateInfo(scheme);
            if (result?.Succeeded == true && result.Principal != null)
                return result;
        }

        return null;
    }
}

