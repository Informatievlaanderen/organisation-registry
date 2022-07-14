namespace OrganisationRegistry.Api.Edit.Organisation.Key;

using System;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using Infrastructure.Swagger;
using Infrastructure.Swagger.Examples;
using OrganisationRegistry.Infrastructure.Commands;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[FeatureGate(FeatureFlags.EditApi)]
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/keys")]
[ApiController]
[ApiExplorerSettings(GroupName = "Organisatiesleutels")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.Keys)]
public class OrganisationKeyController : EditApiController
{
    public OrganisationKeyController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een organisatiesleutel toe.</summary>
    /// <remarks>Voegt een organisatiesleutel toe aan een organisatie.
    /// <br />
    /// Organisatiesleutels van hetzelfde type mogen niet overlappen in tijd.</remarks>
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
        [FromBody] AddOrganisationKeyRequest message)
    {
        if (!TryValidateModel(message))
            return ValidationProblem(ModelState);

        var command = AddOrganisationKeyRequestMapping.Map(organisationId, message);
        await CommandSender.Send(command);

        return CreatedWithLocation(
            nameof(Backoffice.Organisation.Key.OrganisationKeyController.Get),
            new { id = command.OrganisationKeyId });
    }

    /// <summary>Pas een organisatiesleutel aan.</summary>
    /// <param name="organisationId">Id van de organisatie.</param>
    /// <param name="organisationKeyId">Id van de organisatiesleutel.</param>
    /// <param name="message"></param>
    /// <response code="200">Indien de organisatiesleutel succesvol is aangepast.</response>
    /// <response code="400">Indien er validatiefouten zijn.</response>
    [HttpPut("{organisationKeyId:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Put(
        [FromRoute] Guid organisationKeyId,
        [FromRoute] Guid organisationId,
        [FromBody] UpdateOrganisationKeyRequest message)
    {
        if (!TryValidateModel(message))
            return ValidationProblem(ModelState);

        var command = UpdateOrganisationKeyRequestMapping.Map(organisationId, organisationKeyId, message);
        await CommandSender.Send(command);

        return Ok();
    }
}
