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
public class OrganisationCapacityCommandController : OrganisationRegistryCommandController
{
    public OrganisationCapacityCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a capacity for an organisation.</summary>
    /// <response code="201">If the capacity is created, together with the location.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationCapacityRequest message)
    {
        var internalMessage = new AddOrganisationCapacityInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationCapacityRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationCapacityController), nameof(OrganisationCapacityController.Get), new { id = message.OrganisationCapacityId });
    }

    /// <summary>Update a capacity for an organisation.</summary>
    /// <response code="201">If the capacity is updated, together with the location.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationCapacityRequest message)
    {
        var internalMessage = new UpdateOrganisationCapacityInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationCapacityRequestMapping.Map(internalMessage));

        return Ok();
    }

    /// <summary>
    /// Remove an organisation capacity
    /// </summary>
    /// <response code="204">If the organisation capacity is successfully removed.</response>
    /// <response code="400">If the organisation capacity id does not pass validation.</response>
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
