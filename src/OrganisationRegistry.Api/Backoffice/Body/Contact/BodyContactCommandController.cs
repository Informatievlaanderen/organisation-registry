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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyContactCommandController : OrganisationRegistryCommandController
{
    public BodyContactCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Maak een contact aan voor een orgaan.</summary>
    /// <response code="201">Als het contact succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het contact mislukt is.</response>
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

    /// <summary>Pas een contact aan voor een orgaan.</summary>
    /// <response code="200">Als het contact succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het contact mislukt is.</response>
    /// <response code="200">Als het contact succesvol aangepast is.</response>
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
