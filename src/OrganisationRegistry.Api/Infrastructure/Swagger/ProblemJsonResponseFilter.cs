namespace OrganisationRegistry.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Replaces the default content type in 4xx responses with 'application/problem+json'.
    /// </summary>
    public class ProblemJsonResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var (_, value) in operation.Responses.Where(entry =>
                (entry.Key.StartsWith("4") && entry.Key != "400") ||
                entry.Key.StartsWith("5")))
            {
                if (!value.Content.Any())
                    return;

                var openApiMediaType = value.Content.First().Value;

                value.Content.Clear();
                value.Content.Add(
                    new KeyValuePair<string, OpenApiMediaType>("application/problem+json", openApiMediaType));
            }
        }
    }
}
