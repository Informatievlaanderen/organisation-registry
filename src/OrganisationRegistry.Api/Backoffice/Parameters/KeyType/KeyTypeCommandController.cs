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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class KeyTypeCommandController : OrganisationRegistryCommandController
{
    public KeyTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een sleuteltype.</summary>
    /// <response code="201">Als het sleuteltype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het sleuteltype mislukt is.</response>
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

    /// <summary>Pas een sleuteltype aan.</summary>
    /// <response code="200">Als het sleuteltype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het sleuteltype mislukt is.</response>
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

    /// <summary>Verwijder een sleuteltype.</summary>
    /// <response code="204">Als het sleuteltype succesvol verwijderd is.</response>
    /// <response code="400">Als de validatie voor het sleuteltype mislukt is.</response>
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
