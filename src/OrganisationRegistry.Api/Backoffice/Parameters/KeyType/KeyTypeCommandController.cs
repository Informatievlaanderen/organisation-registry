namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType;

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
[OrganisationRegistryRoute("keytypes")]
[OrganisationRegistryAuthorize]
public class KeyTypeCommandController : OrganisationRegistryCommandController
{
    public KeyTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a key type.</summary>
    /// <response code="201">If the key type is created, together with the location.</response>
    /// <response code="400">If the key type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateKeyTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateKeyTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(KeyTypeController), nameof(KeyTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a key type.</summary>
    /// <response code="200">If the key type is updated, together with the location.</response>
    /// <response code="400">If the key type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateKeyTypeRequest message)
    {
        var internalMessage = new UpdateKeyTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateKeyTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(KeyTypeController), nameof(KeyTypeController.Get), new { id = internalMessage.KeyTypeId });
    }

    /// <summary>
    /// Remove a key type
    /// </summary>
    /// <response code="204">If the key type is successfully removed.</response>
    /// <response code="400">If the key type information does not pass validation.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var internalMessage = new RemoveKeyTypeRequest(id);

        await CommandSender.Send(RemoveKeyTypeRequestMapping.Map(internalMessage));

        return NoContent();
    }
}
