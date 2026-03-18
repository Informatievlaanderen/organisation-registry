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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class LocationCommandController : OrganisationRegistryCommandController
{
    public LocationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een locatie.</summary>
    /// <response code="201">Als de locatie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de locatie mislukt is.</response>
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

    /// <summary>Pas een locatie aan.</summary>
    /// <response code="200">Als de locatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de locatie mislukt is.</response>
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
