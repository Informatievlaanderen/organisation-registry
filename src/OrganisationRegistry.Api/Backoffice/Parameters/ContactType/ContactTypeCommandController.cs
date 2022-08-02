namespace OrganisationRegistry.Api.Backoffice.Parameters.ContactType;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("contacttypes")]
public class ContactTypeCommandController : OrganisationRegistryController
{
    public ContactTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a contact type.</summary>
    /// <response code="201">If the contact type is created, together with the location.</response>
    /// <response code="400">If the contact type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateContactTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateContactTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(ContactTypeController),nameof(ContactTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a contact type.</summary>
    /// <response code="200">If the contact type is updated, together with the location.</response>
    /// <response code="400">If the contact type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateContactTypeRequest message)
    {
        var internalMessage = new UpdateContactTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateContactTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(ContactTypeController),nameof(ContactTypeController.Get), new { id = internalMessage.ContactTypeId });
    }
}
