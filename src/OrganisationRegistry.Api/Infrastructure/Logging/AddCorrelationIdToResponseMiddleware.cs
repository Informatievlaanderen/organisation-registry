namespace OrganisationRegistry.Api.Infrastructure.Logging
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class AddCorrelationIdToResponseMiddleware
    {
        private const string CorrelationIdHeaderName = "x-correlation-id";
        private readonly RequestDelegate _next;

        public AddCorrelationIdToResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context
                .Response
                .Headers
                .Add(CorrelationIdHeaderName, context.TraceIdentifier);

            return _next(context);
        }
    }
}
