namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using OrganisationRegistry.Infrastructure.Authorization;

public class ConfigureClaimsPrincipalSelectorMiddleware
{
    private readonly RequestDelegate _next;

    public ConfigureClaimsPrincipalSelectorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context, IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal.ClaimsPrincipalSelector = () =>
        {
            try
            {
                if (context.User.Identity is { IsAuthenticated: true })
                    return AddIpClaim(context, context.User);

                if (TryGetAuthInfo(httpContextAccessor) is not { Principal: { } principal }) return null!;

                return AddIpClaim(context, principal);
            }
            catch
            {
                return null!;
            }
        };

        return _next(context);
    }

    private static ClaimsPrincipal AddIpClaim(HttpContext context, ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity user) return principal;

        var ip = context.Request.HttpContext.Connection.RemoteIpAddress;

        if (!user.HasClaim(x => x.Type == AcmIdmConstants.Claims.Ip))
            user.AddClaim(new Claim(AcmIdmConstants.Claims.Ip, ip?.ToString() ?? "Unknown", ClaimValueTypes.String));

        return principal;
    }

    private static AuthenticateResult? TryGetAuthInfo(IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.GetAuthenticateInfo(AuthenticationSchemes.Introspection) ??
           httpContextAccessor.HttpContext?.GetAuthenticateInfo(AuthenticationSchemes.BffApi) ??
           httpContextAccessor.HttpContext?.GetAuthenticateInfo(JwtBearerDefaults.AuthenticationScheme);
}
