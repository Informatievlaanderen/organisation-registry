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
        // The selector is a process-global static, so it must not capture the current request's
        // HttpContext. Resolve the already-authenticated principal from the request-scoped
        // (AsyncLocal-backed) IHttpContextAccessor instead, which is concurrency-safe under
        // parallel requests. Re-running authentication here (e.g. token introspection) is both
        // unnecessary and flaky under load.
        ClaimsPrincipal.ClaimsPrincipalSelector = () =>
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext?.User.Identity is not { IsAuthenticated: true })
                    return null!;

                var principal = httpContext.User;

                if (principal.Identity is ClaimsIdentity user
                    && !user.HasClaim(x => x.Type == AcmIdmConstants.Claims.Ip))
                {
                    var ip = httpContext.Connection.RemoteIpAddress;
                    user.AddClaim(new Claim(AcmIdmConstants.Claims.Ip, ip?.ToString() ?? "Unknown", ClaimValueTypes.String));
                }

                return principal;
            }
            catch
            {
                return null!;
            }
        };

        return _next(context);
    }
}
