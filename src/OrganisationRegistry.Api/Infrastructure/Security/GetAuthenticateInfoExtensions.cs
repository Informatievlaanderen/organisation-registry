namespace OrganisationRegistry.Api.Infrastructure.Security
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;

    public static class GetAuthenticateInfoExtensions
    {
        public static async Task<AuthenticateResult?> GetAuthenticateInfoAsync(this HttpContext source)
        {
            var bearerInfo = await source.GetBearerAuthenticateInfo();
            return bearerInfo is { Succeeded: true } ? bearerInfo : null;
        }

        public static AuthenticateResult? GetAuthenticateInfo(this HttpContext source)
        {
            var bearerInfo = source.GetBearerAuthenticateInfo().GetAwaiter().GetResult();
            return bearerInfo is { Succeeded: true } ? bearerInfo : null;
        }

        private static Task<AuthenticateResult> GetBearerAuthenticateInfo(this HttpContext source)
            => source.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    }
}
