namespace OrganisationRegistry.Api.Backoffice.Parameters.SeatType;

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
[OrganisationRegistryRoute("seattypes")]
[OrganisationRegistryAuthorize]
public class SeatTypeCommandController : OrganisationRegistryCommandController
{
    public SeatTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a seat type.</summary>
    /// <response code="201">If the seat type is created, together with the location.</response>
    /// <response code="400">If the seat type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateSeatTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateSeatTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(SeatTypeController), nameof(SeatTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a seat type.</summary>
    /// <response code="200">If the seat type is updated, together with the location.</response>
    /// <response code="400">If the seat type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateSeatTypeRequest message)
    {
        var internalMessage = new UpdateSeatTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateSeatTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(SeatTypeController), nameof(SeatTypeController.Get), new { id = internalMessage.SeatTypeId });
    }
}
