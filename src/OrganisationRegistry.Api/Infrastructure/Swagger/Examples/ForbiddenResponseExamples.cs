namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

[DataContract]
public class ForbiddenResponseExamples : IExamplesProvider<ProblemDetails>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public ForbiddenResponseExamples(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _problemDetailsHelper = problemDetailsHelper;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            ProblemTypeUri = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = "Forbidden",
            Detail = "",
            ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext!)
        };
}
