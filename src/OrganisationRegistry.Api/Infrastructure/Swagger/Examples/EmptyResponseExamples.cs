namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using Swashbuckle.AspNetCore.Filters;

public class EmptyResponseExamples : IExamplesProvider<object>
{
    public object GetExamples()
        => null!;
}