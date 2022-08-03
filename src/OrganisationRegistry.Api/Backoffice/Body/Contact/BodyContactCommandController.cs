namespace OrganisationRegistry.Api.Backoffice.Body.Contact;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/contacts")]
[OrganisationRegistryAuthorize]
public class BodyContactCommandController : OrganisationRegistryCommandController
{
    public BodyContactCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a contact for an organisation.</summary>
    /// <response code="201">If the contact is created, together with the location.</response>
    /// <response code="400">If the contact information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyContactRequest message)
    {
        var internalMessage = new AddBodyContactInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyContactRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodyContactController), nameof(BodyContactController.Get), new { id = message.BodyContactId });
    }

    /// <summary>Update a contact for an organisation.</summary>
    /// <response code="201">If the contact is updated, together with the location.</response>
    /// <response code="400">If the contact information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyContactRequest message)
    {
        var internalMessage = new UpdateBodyContactInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyContactRequestMapping.Map(internalMessage));

        return Ok();
    }
}
