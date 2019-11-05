namespace OrganisationRegistry.Api.Infrastructure.Security
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class AddHttpSecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public AddHttpSecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            context.Response.Headers.Add("x-powered-by", "Vlaamse overheid");
            context.Response.Headers.Add("x-content-type-options", "nosniff");
            context.Response.Headers.Add("x-frame-options", "DENY");
            context.Response.Headers.Add("x-xss-protection", "1; mode=block");

            context.Response.Headers.Add("cache-control", "no-store, no-cache, must-revalidate");
            context.Response.Headers.Add("pragma", "no-cache");

            return _next(context);
        }
    }
}
