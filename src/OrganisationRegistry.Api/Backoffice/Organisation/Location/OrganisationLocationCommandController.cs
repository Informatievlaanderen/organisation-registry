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
public class OrganisationLocationCommandController : OrganisationRegistryCommandController
{
    public OrganisationLocationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Add a location to an organisation.</summary>
    /// <response code="201">If the location is added, together with the location.</response>
    /// <response code="400">If the location information does not pass validation.</response>
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

    /// <summary>Update a location for an organisation.</summary>
    /// <response code="201">If the location is updated, together with the location.</response>
    /// <response code="400">If the location information does not pass validation.</response>
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

    /// <summary>Remove a location for an organisation.</summary>
    /// <response code="204">If the location is removed.</response>
    /// <response code="400">If the location information does not pass validation.</response>
    [HttpDelete("{organisationLocationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid organisationLocationId)
    {
        await CommandSender.Send(new DeleteOrganisationLocation(new OrganisationId(organisationId), organisationLocationId));

        return Ok();
    }
}
