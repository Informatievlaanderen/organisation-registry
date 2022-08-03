namespace OrganisationRegistry.Api.Backoffice.Body.Seat;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/seats")]
[OrganisationRegistryAuthorize]
public class BodySeatCommandController : OrganisationRegistryCommandController
{
    public BodySeatCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a seat for a body.</summary>
    /// <response code="201">If the seat is created, together with the location.</response>
    /// <response code="400">If the seat information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodySeatRequest message)
    {
        var internalMessage = new AddBodySeatInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodySeatRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodySeatController), nameof(BodySeatController.Get), new { id = message.BodySeatId });
    }

    /// <summary>Update a seat for a body.</summary>
    /// <response code="201">If the seat is updated, together with the location.</response>
    /// <response code="400">If the seat information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodySeatRequest message)
    {
        var internalMessage = new UpdateBodySeatInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodySeatRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodySeatController), nameof(BodySeatController.Get), new { id = internalMessage.BodyId });
    }
}
