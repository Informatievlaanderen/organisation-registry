namespace OrganisationRegistry.Api.Edit.Organisation.Classification;

using System;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Infrastructure;
using Infrastructure.Security;
using Infrastructure.Swagger;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using Swashbuckle.AspNetCore.Filters;

[FeatureGate(FeatureFlags.EditApi)]
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/classifications")]
[ApiController]
[ApiExplorerSettings(GroupName = "Organisatieclassificaties")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.OrganisationClassifications)]
public class OrganisationOrganisationClassificationController : EditApiController
{
    public OrganisationOrganisationClassificationController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Voeg een organisatieclassificate toe.</summary>
    /// <remarks>Voegt een organisatieclassificate toe aan een organisatie.
    /// <br />
    /// Dezelfde organisatieclassificatie mag niet overlappen in tijd met eenzelfde organisatieclassificatie binnen een organisatie.</remarks>
    /// <param name="organisationId">Id van de organisatie.</param>
    /// <param name="message"></param>
    /// <response code="201">Als het verzoek aanvaard is.</response>
    /// <response code="400">Als het verzoek ongeldige data bevat.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(EmptyResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerLocationHeader]
    public async Task<IActionResult> Post(
        [FromRoute] Guid organisationId,
        [FromBody] AddOrganisationOrganisationClassificationRequest message)
    {
        if (!TryValidateModel(message))
            return ValidationProblem(ModelState);

        var command = AddOrganisationOrganisationClassificationRequestMapping.Map(organisationId, message);
        await CommandSender.Send(command);

        return await Task.FromResult(
            CreatedWithLocation(
                nameof(Backoffice.Organisation.OrganisationClassification.OrganisationOrganisationClassificationController),
                nameof(Backoffice.Organisation.OrganisationClassification.OrganisationOrganisationClassificationController.Get),
                new { organisationId = organisationId, id = command.OrganisationOrganisationClassificationId }));
    }

    /// <summary>Pas een organisatiesleutel aan.</summary>
    /// <remarks>Past een bankrekeningnummer aan voor een organisatie.
    /// <br />
    /// Hetzelfde bankrekeningnummer mag niet overlappen in tijd met eenzelfde bankrekeningnummer binnen een organisatie.</remarks>
    /// <param name="organisationId">Id van de organisatie.</param>
    /// <param name="organisationOrganisationClassificationId">Id van de organisatiesleutel.</param>
    /// <param name="message"></param>
    /// <response code="200">Indien de organisatiesleutel succesvol is aangepast.</response>
    /// <response code="400">Indien er validatiefouten zijn.</response>
    [HttpPut("{organisationOrganisationClassificationId:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Put(
        [FromRoute] Guid organisationOrganisationClassificationId,
        [FromRoute] Guid organisationId,
        [FromBody] UpdateOrganisationOrganisationClassificationRequest message)
    {
        if (!TryValidateModel(message))
            return ValidationProblem(ModelState);

        var command = UpdateOrganisationOrganisationClassificationRequestMapping.Map(organisationId, organisationOrganisationClassificationId, message);
        await CommandSender.Send(command);

        return Ok();
    }
}
