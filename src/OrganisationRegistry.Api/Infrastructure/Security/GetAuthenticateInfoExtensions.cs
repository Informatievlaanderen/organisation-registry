namespace OrganisationRegistry.Api.Infrastructure.Security
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;

    public static class GetAuthenticateInfoExtensions
    {
        public static async Task<AuthenticateResult> GetAuthenticateInfoAsync(this HttpContext source)
        {
            var bearerInfo = await source.GetBearerAuthenticateInfo();
            if (bearerInfo != null && bearerInfo.Succeeded)
                return bearerInfo;

            return null;
        }

        public static AuthenticateResult GetAuthenticateInfo(this HttpContext source)
        {
            var bearerInfo = source.GetBearerAuthenticateInfo().Result;
            if (bearerInfo != null && bearerInfo.Succeeded)
                return bearerInfo;

            return null;
        }

        private static Task<AuthenticateResult> GetBearerAuthenticateInfo(this HttpContext source)
        {
            return source.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        }
    }
}
