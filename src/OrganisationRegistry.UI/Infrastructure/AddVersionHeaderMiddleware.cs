namespace OrganisationRegistry.UI.Infrastructure
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class AddVersionHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public AddVersionHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            context.Response.Headers.Add("x-wegwijs-version", version);

            return _next(context);
        }
    }
}
