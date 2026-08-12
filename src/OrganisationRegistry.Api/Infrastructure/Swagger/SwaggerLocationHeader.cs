namespace OrganisationRegistry.Api.Infrastructure.Swagger;

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

public class SwaggerLocationHeaderAttribute : SwaggerResponseHeaderAttribute
{
    public SwaggerLocationHeaderAttribute(): base(Microsoft.AspNetCore.Http.StatusCodes.Status201Created, "Location", JsonSchemaType.String, "Uri van het aangemaakt object.")
    {
    }
}
