namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using Edit.Organisation.KboNumber;
using Swashbuckle.AspNetCore.Filters;

public class EmptyResponseExamples : IExamplesProvider<object>
{
    public object GetExamples()
        => null!;
}

public class OvoNumberResponseExamples : IExamplesProvider<object>
{
    public object GetExamples()
        => new CreateOrganisationByKboNumberResponse("OVO099999");
}
