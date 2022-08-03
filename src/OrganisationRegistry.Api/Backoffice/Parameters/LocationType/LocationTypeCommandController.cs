namespace OrganisationRegistry.Api.Backoffice.Parameters.LocationType;

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
[OrganisationRegistryRoute("locationtypes")]
[OrganisationRegistryAuthorize]
public class LocationTypeCommandController : OrganisationRegistryCommandController
{
    public LocationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a location type.</summary>
    /// <response code="201">If the location type is created, together with the location.</response>
    /// <response code="400">If the location type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateLocationTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateLocationTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(LocationTypeController), nameof(LocationTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a location type.</summary>
    /// <response code="200">If the location type is updated, together with the location.</response>
    /// <response code="400">If the location type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLocationTypeRequest message)
    {
        var internalMessage = new UpdateLocationTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateLocationTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(LocationTypeController), nameof(LocationTypeController.Get), new { id = internalMessage.LocationTypeId });
    }
}
