namespace OrganisationRegistry.Api;

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using OrganisationRegistry.Infrastructure.Authorization;

public static class HttpContextAccessorExtensions
{
    public static async Task<bool> UserIsDecentraalBeheerder(this IHttpContextAccessor httpContextAccessor, Func<ClaimsPrincipal?, Task<SecurityInformation>> getSecurityInformation)
    {
        var maybeHttpContext = httpContextAccessor.HttpContext;
        if (maybeHttpContext is not { } httpContext)
            throw new NullReferenceException("httpContext should not be null");

        var maybeAuthenticateInfo = await httpContext.GetAuthenticateInfoAsync();
        if (maybeAuthenticateInfo is not { } authenticateInfo)
            throw new NullReferenceException("authenticateInfo should not be null");

        return (await getSecurityInformation(authenticateInfo.Principal))
            .Roles.Contains(Role.DecentraalBeheerder);
    }
}
