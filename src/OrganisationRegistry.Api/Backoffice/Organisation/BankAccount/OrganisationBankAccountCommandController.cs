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
public class OrganisationBankAccountCommandController : OrganisationRegistryCommandController
{
    public OrganisationBankAccountCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Add a bankAccount to an organisation.</summary>
    /// <response code="201">If the bankAccount is added, together with the location.</response>
    /// <response code="400">If the bankAccount information does not pass validation.</response>
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

    /// <summary>Update a bankAccount for an organisation.</summary>
    /// <response code="201">If the bankAccount is updated, together with the location.</response>
    /// <response code="400">If the bankAccount information does not pass validation.</response>
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

    /// <summary>Remove a bankaccount from an organisation</summary>
    /// <response code="204">If the bankAccount is removed.</response>
    /// <response code="400">If the bankAccount is not found for the organisation.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        await CommandSender.Send(new RemoveOrganisationBankAccount(new OrganisationId(organisationId), id));
        return NoContent();
    }
}
