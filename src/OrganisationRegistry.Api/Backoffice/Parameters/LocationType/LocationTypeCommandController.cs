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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class LocationTypeCommandController : OrganisationRegistryCommandController
{
    public LocationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een locatietype.</summary>
    /// <response code="201">Als het locatietype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het locatietype mislukt is.</response>
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

    /// <summary>Pas een locatietype aan.</summary>
    /// <response code="200">Als het locatietype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het locatietype mislukt is.</response>
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
