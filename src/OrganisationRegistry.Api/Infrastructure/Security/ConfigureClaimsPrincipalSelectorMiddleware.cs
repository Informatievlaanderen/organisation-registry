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

    private static AuthenticateResult? TryGetAuthInfo(IHttpContextAccessor httpContextAccessor)
    {
        var authInfo = httpContextAccessor.HttpContext?.GetAuthenticateInfo(JwtBearerDefaults.AuthenticationScheme) ??
                       httpContextAccessor.HttpContext?.GetAuthenticateInfo(AuthenticationSchemes.EditApi);
        return authInfo;
    }
}
