namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Security.Claims;
using System.Threading.Tasks;
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
                var authInfo = httpContextAccessor.HttpContext?.GetAuthenticateInfo();
                if (authInfo?.Principal == null)
                    return null!;
                if (!(authInfo.Principal.Identity is ClaimsIdentity user))
                    return authInfo.Principal;

                var ip = context.Request.HttpContext.Connection.RemoteIpAddress;

                if (!user.HasClaim(x => x.Type == AcmIdmConstants.Claims.Ip))
                    user.AddClaim(new Claim(AcmIdmConstants.Claims.Ip, ip?.ToString() ?? "Unknown", ClaimValueTypes.String));

                return authInfo.Principal;
            }
            catch
            {
                return null!;
            }
        };

        return _next(context);
    }
}