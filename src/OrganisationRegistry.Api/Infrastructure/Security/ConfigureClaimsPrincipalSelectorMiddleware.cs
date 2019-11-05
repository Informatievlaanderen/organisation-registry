namespace OrganisationRegistry.Api.Infrastructure.Security
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

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
                    var authInfo = httpContextAccessor.HttpContext.GetAuthenticateInfo();
                    if (authInfo?.Principal == null)
                        return null;
                    if (!(authInfo.Principal.Identity is ClaimsIdentity user))
                        return authInfo.Principal;

                    var ip = context.Request.HttpContext.Connection.RemoteIpAddress;

                    const string urnBeVlaanderenOrganisationRegistryIp = "urn:be:vlaanderen:wegwijs:ip";
                    if (!user.HasClaim(x => x.Type == urnBeVlaanderenOrganisationRegistryIp))
                        user.AddClaim(new Claim(urnBeVlaanderenOrganisationRegistryIp, ip.ToString(), ClaimValueTypes.String));

                    return authInfo.Principal;
                }
                catch
                {
                    return null;
                }
            };

            return _next(context);
        }
    }
}
