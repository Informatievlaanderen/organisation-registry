namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class GetAuthenticateInfoExtensions
{
    public static async Task<AuthenticateResult?> GetAuthenticateInfoAsync(this HttpContext source)
    {
        var bearerInfo = await source.GetAuthenticateInfoIfRegisteredAsync(JwtBearerDefaults.AuthenticationScheme);
        if (bearerInfo is { Succeeded: true })
            return bearerInfo;

        var bffApiInfo = await source.GetAuthenticateInfoIfRegisteredAsync(AuthenticationSchemes.BffApi);
        return bffApiInfo is { Succeeded: true } ? bffApiInfo : null;
    }

    public static AuthenticateResult? GetAuthenticateInfo(this HttpContext source, string authenticationScheme)
    {
        var bearerInfo = source.GetAuthenticateInfoIfRegisteredAsync(authenticationScheme).GetAwaiter().GetResult();
        return bearerInfo is { Succeeded: true } ? bearerInfo : null;
    }

    private static async Task<AuthenticateResult?> GetAuthenticateInfoIfRegisteredAsync(
        this HttpContext source,
        string authenticationScheme)
    {
        var schemeProvider = source.RequestServices.GetService<IAuthenticationSchemeProvider>();
        if (schemeProvider == null || await schemeProvider.GetSchemeAsync(authenticationScheme) == null)
            return null;

        return await source.AuthenticateAsync(authenticationScheme);
    }
}
