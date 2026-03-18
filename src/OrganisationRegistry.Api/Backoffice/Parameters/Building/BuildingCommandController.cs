namespace OrganisationRegistry.Api.Backoffice.Parameters.Building;

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
[OrganisationRegistryRoute("buildings")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class BuildingCommandController : OrganisationRegistryCommandController
{
    public BuildingCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een gebouw.</summary>
    /// <response code="201">Als het gebouw succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het gebouw mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBuildingRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateBuildingRequestMapping.Map(message));

        return CreatedWithLocation(nameof(BuildingController),nameof(BuildingController.Get), new { id = message.Id });
    }

    /// <summary>Pas een gebouw aan.</summary>
    /// <response code="200">Als het gebouw succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het gebouw mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBuildingRequest message)
    {
        var internalMessage = new UpdateBuildingInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBuildingRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BuildingController),nameof(BuildingController.Get), new { id = internalMessage.BuildingId });
    }
}
