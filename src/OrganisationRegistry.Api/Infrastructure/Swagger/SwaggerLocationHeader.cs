namespace OrganisationRegistry.Api.Infrastructure.Swagger
{
    using Swashbuckle.AspNetCore.Filters;

    public class SwaggerLocationHeaderAttribute : SwaggerResponseHeaderAttribute
    {
        public SwaggerLocationHeaderAttribute(): base(Microsoft.AspNetCore.Http.StatusCodes.Status201Created, "Location", "string", "Uri van het aangemaakt object.")
        {
        }
    }
}
