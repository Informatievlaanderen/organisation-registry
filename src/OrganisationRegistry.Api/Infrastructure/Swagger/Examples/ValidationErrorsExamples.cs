namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using System.Runtime.Serialization;
using Swashbuckle.AspNetCore.Filters;

[DataContract]
public class ValidationErrorsExamples : IExamplesProvider<ValidationErrors>
{
    public ValidationErrors GetExamples()
        => new()
        {
            { "Body.ValidTo", new[] { "Valid To must be greater than or equal to Valid From." } },
        };
}
