namespace OrganisationRegistry.Api.Edit.Organisation
{
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using OrganisationRegistry.Infrastructure.Commands;
    using Swashbuckle.AspNetCore.Filters;
    using AuthenticationSchemes = Infrastructure.Security.AuthenticationSchemes;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [FeatureGate(FeatureFlags.EditApi)]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("edit")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Organisatiesleutels")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class OrganisationTestAuthController : OrganisationRegistryController
    {
        public OrganisationTestAuthController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>
        /// Test de authenticatie.
        /// </summary>
        /// <remarks>Tijdelijk endpoint bedoeld om de authenticatie te testen.</remarks>
        /// <returns></returns>
        [HttpGet("auth")]
        [Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExamples))]
        public async Task<IActionResult> TestAuthorization()
        {
            return Ok();
        }
    }
}
