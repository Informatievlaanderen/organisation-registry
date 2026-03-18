namespace OrganisationRegistry.Api.Backoffice.Organisation.Location;

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
[OrganisationRegistryRoute("organisations/{organisationId}/locations")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationLocationCommandController : OrganisationRegistryCommandController
{
    public OrganisationLocationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een locatie toe aan een organisatie.</summary>
    /// <response code="201">Als de locatie succesvol toegevoegd is.</response>
    /// <response code="400">Als de validatie voor de locatie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationLocationRequest message)
    {
        var internalMessage = new AddOrganisationLocationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationLocationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationLocationController), nameof(OrganisationLocationController.Get), new { id = message.OrganisationLocationId });
    }

    /// <summary>Pas een locatie aan voor een organisatie.</summary>
    /// <response code="201">Als de locatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de locatie mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationLocationRequest message)
    {
        var internalMessage = new UpdateOrganisationLocationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationLocationRequestMapping.Map(internalMessage));

        return Ok();
    }

    /// <summary>Verwijder een locatie van een organisatie.</summary>
    /// <response code="204">Als de locatie succesvol verwijderd is.</response>
    /// <response code="400">Als de validatie voor de locatie mislukt is.</response>
    [HttpDelete("{organisationLocationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid organisationLocationId)
    {
        await CommandSender.Send(new DeleteOrganisationLocation(new OrganisationId(organisationId), organisationLocationId));

        return NoContent();
    }
}
