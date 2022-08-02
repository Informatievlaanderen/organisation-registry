namespace OrganisationRegistry.Api.Backoffice.Parameters.Location;

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
[OrganisationRegistryRoute("locations")]
[OrganisationRegistryAuthorize]
public class LocationCommandController : OrganisationRegistryController
{
    public LocationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a location.</summary>
    /// <response code="201">If the location is created, together with the location.</response>
    /// <response code="400">If the location information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateLocationRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateLocationRequestMapping.Map(message);
        await CommandSender.Send(command);

        return CreatedWithLocation(nameof(LocationController), nameof(LocationController.Get), new { id = command.Id });
    }

    /// <summary>Update a location.</summary>
    /// <response code="200">If the location is updated, together with the location.</response>
    /// <response code="400">If the location information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLocationRequest message)
    {
        var internalMessage = new UpdateLocationInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateLocationRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(LocationController), nameof(LocationController.Get), new { id = internalMessage.LocationId });
    }
}
