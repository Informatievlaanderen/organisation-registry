namespace OrganisationRegistry.Api.Backoffice.Organisation.Capacity;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("organisations/{organisationId}/capacities")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationCapacityCommandController : OrganisationRegistryCommandController
{
    public OrganisationCapacityCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een hoedanigheid toe aan een organisatie.</summary>
    /// <response code="201">Als de hoedanigheid succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de hoedanigheid mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationCapacityRequest message)
    {
        if (!TryValidateModel(message))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationCapacityRequestMapping.Map(organisationId, message));

        return CreatedWithLocation(nameof(OrganisationCapacityController), nameof(OrganisationCapacityController.Get), new { id = message.OrganisationCapacityId });
    }

    /// <summary>Pas een hoedanigheid aan voor een organisatie.</summary>
    /// <response code="200">Als de hoedanigheid succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de hoedanigheid mislukt is.</response>
    /// <response code="200">Als de hoedanigheid succesvol aangepast is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationCapacityRequest message)
    {
        if (!TryValidateModel(message))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationCapacityRequestMapping.Map(organisationId, message));

        return Ok();
    }

    /// <summary>Verwijder een hoedanigheid van een organisatie.</summary>
    /// <response code="204">Als de organisatiecapaciteit succesvol verwijderd is.</response>
    /// <response code="400">Als de validatie voor de organisatiecapaciteit mislukt is.</response>
    [HttpDelete("{organisationCapacityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid organisationCapacityId)
    {
        var internalMessage = new RemoveOrganisationCapacityRequest(organisationId, organisationCapacityId);

        await CommandSender.Send(RemoveOrganisationCapacityRequestMapping.Map(internalMessage));

        return NoContent();
    }
}
