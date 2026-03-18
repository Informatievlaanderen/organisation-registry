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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationContactCommandController : OrganisationRegistryCommandController
{
    public OrganisationContactCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een contact toe aan een organisatie.</summary>
    /// <response code="201">Als het contact succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het contact mislukt is.</response>
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

    /// <summary>Pas een contact aan voor een organisatie.</summary>
    /// <response code="200">Als het contact succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het contact mislukt is.</response>
    /// <response code="200">Als het contact succesvol aangepast is.</response>
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
