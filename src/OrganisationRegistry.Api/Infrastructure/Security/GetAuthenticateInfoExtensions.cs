namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

public static class GetAuthenticateInfoExtensions
{
    public static async Task<AuthenticateResult?> GetAuthenticateInfoAsync(this HttpContext source)
    {
        var bearerInfo = await source.GetBearerAuthenticateInfo(JwtBearerDefaults.AuthenticationScheme);
        return bearerInfo is { Succeeded: true } ? bearerInfo : null;
    }

    public static AuthenticateResult? GetAuthenticateInfo(this HttpContext source, string authenticationScheme)
    {
        var bearerInfo = source.GetBearerAuthenticateInfo(authenticationScheme).GetAwaiter().GetResult();
        return bearerInfo is { Succeeded: true } ? bearerInfo : null;
    }

    private static Task<AuthenticateResult> GetBearerAuthenticateInfo(this HttpContext source, string authenticationScheme)
        => source.AuthenticateAsync(authenticationScheme);
}
