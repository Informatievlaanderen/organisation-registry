namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity;

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
[OrganisationRegistryRoute("capacities")]
public class CapacityCommandController : OrganisationRegistryController
{
    public CapacityCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a capacity.</summary>
    /// <response code="201">If the capacity is created, together with the location.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateCapacityRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateCapacityRequestMapping.Map(message));

        return CreatedWithLocation(nameof(CapacityController),nameof(CapacityController.Get), new { id = message.Id });
    }

    /// <summary>Update a capacity.</summary>
    /// <response code="200">If the capacity is updated, together with the location.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCapacityRequest message)
    {
        var internalMessage = new UpdateCapacityInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateCapacityRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(CapacityController),nameof(CapacityController.Get), new { id = internalMessage.CapacityId });
    }

    /// <summary>
    /// Remove a capacity
    /// </summary>
    /// <response code="204">If the capacity is successfully removed.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var internalMessage = new RemoveCapacityRequest(id);

        await CommandSender.Send(RemoveCapacityRequestMapping.Map(internalMessage));

        return NoContent();
    }
}
