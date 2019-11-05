namespace OrganisationRegistry.Api.Infrastructure.Logging
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog.Context;

    public class AddCorrelationIdToLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public AddCorrelationIdToLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
                return _next(context);
        }
    }
}
