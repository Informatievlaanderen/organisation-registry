namespace OrganisationRegistry.Api.Edit.Organisation.BankAccount;

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
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[FeatureGate(FeatureFlags.EditApi)]
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/bankaccounts")]
[ApiController]
[ApiExplorerSettings(GroupName = "Organisatiebankrekeningnummers")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.BankAccounts)]
public class OrganisationBankAccountController : EditApiController
{
    public OrganisationBankAccountController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Voeg een bankrekeningnummer toe.</summary>
    /// <remarks>Voegt een bankrekeningnummer toe aan een organisatie.
    /// <br />
    /// Hetzelfde bankrekeningnummer mag niet overlappen in tijd met eenzelfde bankrekeningnummer binnen een organisatie.</remarks>
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
        [FromBody] AddOrganisationBankAccountRequest message)
    {
        if (!TryValidateModel(message))
            return ValidationProblem(ModelState);

        var command = AddOrganisationBankAccountRequestMapping.Map(organisationId, message);
        await CommandSender.Send(command);

        return CreatedWithLocation(
            nameof(Backoffice.Organisation.BankAccount.OrganisationBankAccountController.Get),
            new { organisationId = organisationId, id = command.OrganisationBankAccountId });
    }

    /// <summary>Pas een organisatiesleutel aan.</summary>
    /// <remarks>Past een bankrekeningnummer aan voor een organisatie.
    /// <br />
    /// Hetzelfde bankrekeningnummer mag niet overlappen in tijd met eenzelfde bankrekeningnummer binnen een organisatie.</remarks>
    /// <param name="organisationId">Id van de organisatie.</param>
    /// <param name="organisationBankAccountId">Id van de organisatiesleutel.</param>
    /// <param name="message"></param>
    /// <response code="200">Indien de organisatiesleutel succesvol is aangepast.</response>
    /// <response code="400">Indien er validatiefouten zijn.</response>
    [HttpPut("{organisationBankAccountId:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Put(
        [FromRoute] Guid organisationBankAccountId,
        [FromRoute] Guid organisationId,
        [FromBody] UpdateOrganisationBankAccountRequest message)
    {
        if (!TryValidateModel(message))
            return ValidationProblem(ModelState);

        var command = UpdateOrganisationBankAccountRequestMapping.Map(organisationId, organisationBankAccountId, message);
        await CommandSender.Send(command);

        return Ok();
    }
}
