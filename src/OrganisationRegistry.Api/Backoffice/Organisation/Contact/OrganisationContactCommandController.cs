namespace OrganisationRegistry.Api.Backoffice.Organisation.Contact;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/contacts")]
[OrganisationRegistryAuthorize]
public class OrganisationContactCommandController : OrganisationRegistryCommandController
{
    public OrganisationContactCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a contact for an organisation.</summary>
    /// <response code="201">If the contact is created, together with the location.</response>
    /// <response code="400">If the contact information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationContactRequest message)
    {
        var internalMessage = new AddOrganisationContactInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationContactRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationContactController), nameof(OrganisationContactController.Get), new { id = message.OrganisationContactId });
    }

    /// <summary>Update a contact for an organisation.</summary>
    /// <response code="201">If the contact is updated, together with the location.</response>
    /// <response code="400">If the contact information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationContactRequest message)
    {
        var internalMessage = new UpdateOrganisationContactInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationContactRequestMapping.Map(internalMessage));

        return Ok();
    }
}
