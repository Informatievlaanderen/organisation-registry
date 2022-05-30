namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrganisationRegistry.Infrastructure.Configuration;

public class ApplicationStatusMiddleware
{
    private readonly RequestDelegate _next;

    public ApplicationStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IOptions<TogglesConfigurationSection> configuration)
    {
        // These routes are always available
        if (context.Request.Path.ToString().StartsWith("/v1/security") ||
            context.Request.Path.ToString().StartsWith("/v1/status") ||
            context.Request.Path.ToString().StartsWith("/v1/configuration") ||
            context.Request.Path.ToString().StartsWith("/v1/projections") ||
            context.Request.Path.ToString().StartsWith("/v1/tasks"))
        {
            await _next(context);
            return;
        }

        // Only work when both OrganisationRegistry and the API is available
        if (configuration.Value.ApplicationAvailable && configuration.Value.ApiAvailable)
        {
            await _next(context);
            return;
        }

        // We are offline!
        context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = ((int)HttpStatusCode.ServiceUnavailable).ToString(),
            Title = "Toepassing is momenteel niet beschikbaar!",
            Detail = "De toepassing is momenteel offline, probeer het later opnieuw."
        }));
    }
}