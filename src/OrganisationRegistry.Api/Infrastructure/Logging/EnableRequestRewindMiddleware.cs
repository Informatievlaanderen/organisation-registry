namespace OrganisationRegistry.Api.Infrastructure.Logging
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;

    public class EnableRequestRewindMiddleware
    {
        private readonly RequestDelegate _next;

        public EnableRequestRewindMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Request.EnableRewind();
            return _next(context);
        }
    }
}
