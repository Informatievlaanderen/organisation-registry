namespace OrganisationRegistry.Api.Backoffice.Organisation.BankAccount;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/bankAccounts")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationBankAccountCommandController : OrganisationRegistryCommandController
{
    public OrganisationBankAccountCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een bankrekeningnummer toe aan een organisatie.</summary>
    /// <response code="201">Als de bankrekening succesvol toegevoegd is.</response>
    /// <response code="400">Als de validatie voor de bankrekening mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationBankAccountRequest message)
    {
        var internalMessage = new AddOrganisationBankAccountInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationBankAccountRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationBankAccountController), nameof(OrganisationBankAccountController.Get), new { id = message.OrganisationBankAccountId });
    }

    /// <summary>Pas een bankrekeningnummer aan voor een organisatie.</summary>
    /// <response code="201">Als de bankrekening succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de bankrekening mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationBankAccountRequest message)
    {
        var internalMessage = new UpdateOrganisationBankAccountInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationBankAccountRequestMapping.Map(internalMessage));

        return Ok();
    }

    /// <summary>Verwijder een bankrekeningnummer van een organisatie.</summary>
    /// <response code="204">Als de bankrekening succesvol verwijderd is.</response>
    /// <response code="400">Als de bankrekening niet gevonden kan worden voor de organisatie.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        await CommandSender.Send(new RemoveOrganisationBankAccount(new OrganisationId(organisationId), id));
        return NoContent();
    }
}
