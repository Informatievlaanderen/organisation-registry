namespace OrganisationRegistry.Api.Edit.Organisation.KboNumber;

using System;
using System.Threading.Tasks;
using Backoffice.Organisation.Detail;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Infrastructure.Security;
using Infrastructure.Swagger;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;
using OrganisationRegistry.SqlServer.Infrastructure;
using SqlServer.Organisation;
using Swashbuckle.AspNetCore.Filters;

[FeatureGate(FeatureFlags.EditApi)]
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("edit/organisations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Organisaties")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.Organisations)]
public class OrganisationsController : OrganisationRegistryController
{
    public OrganisationsController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Maak een organisatie aan op basis van kbonummer.</summary>
    /// <remarks>Maakt een organisatie aan op basis van kbonummer.
    /// <br />
    /// De gegevens voor de organisatie worden opgehaald uit de KBO.
    /// <br />
    /// Indien er reeds een organisatie bestaat met dit kbonummer,
    /// wordt het ovonummer hiervan teruggegeven.
    /// <br />
    /// Indien er een nieuwe organisatie wordt gecreëerd,
    /// wordt het nieuwe ovonummer teruggegeven.</remarks>
    /// <param name="context"></param>
    /// <param name="ovoNumberGenerator"></param>
    /// <param name="kboNumber">Kbonummer van de organisatie.</param>
    /// <returns>Het ovonummer van de organisatie.</returns>
    /// <response code="201">Als het verzoek aanvaard is.</response>
    /// <response code="400">Als het verzoek ongeldige data bevat.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPut("kbo/{kboNumber}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(OvoNumberResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OvoNumberResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerLocationHeader]
    public async Task<IActionResult> Put([FromServices] OrganisationRegistryContext context, [FromServices] IOvoNumberGenerator ovoNumberGenerator, [FromRoute] string kboNumber)
    {
        var maybeOrganisation = await context.OrganisationDetail.SingleOrDefaultAsync(x => x.KboNumber == kboNumber);

        if (maybeOrganisation is { } organisation)
            return ExistingOrganisationResponse(organisation);

        var ovoNumber = ovoNumberGenerator.GenerateNextNumber();
        var organisationId = OrganisationId.New();
        var command = new CreateOrganisationFromKboNumber(organisationId, new KboNumber(kboNumber), ovoNumber);

        await CommandSender.Send(command);

        return await CreatedResponse(organisationId, ovoNumber);
    }

    private IActionResult ExistingOrganisationResponse(OrganisationDetailItem organisation)
    {
        var foundLocation = Action<OrganisationDetailController>(
            nameof(OrganisationDetailController.Get),
            new { id = organisation.Id })!;
        var foundResponse = new CreateOrganisationByKboNumberResponse(organisation.OvoNumber);
        return OkValueWithLocationHeader(foundLocation, foundResponse);
    }

    private async Task<IActionResult> CreatedResponse(Guid organisationId, string ovoNumber)
    {
        var createdLocation = Action<OrganisationDetailController>(
            nameof(OrganisationDetailController.Get),
            new { id = organisationId })!;
        var createdResponse = new CreateOrganisationByKboNumberResponse(ovoNumber);
        return await CreatedAsync(createdLocation, createdResponse);
    }
}
