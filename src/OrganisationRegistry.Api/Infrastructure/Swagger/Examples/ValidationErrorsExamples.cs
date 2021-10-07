namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples
{
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Swashbuckle.AspNetCore.Filters;

    [DataContract]
    public class ValidationErrorsExamples : IExamplesProvider<ValidationErrors>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProblemDetailsHelper _problemDetailsHelper;

        public ValidationErrorsExamples(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _problemDetailsHelper = problemDetailsHelper;
        }

        public ValidationErrors GetExamples()
            => new ValidationErrors
            {
                { "Body.ValidTo", new[] { "Valid To must be greater than or equal to Valid From." } }
            };
    }
}
