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
public class BuildingCommandController : OrganisationRegistryCommandController
{
    public BuildingCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a building.</summary>
    /// <response code="201">If the building is created, together with the location.</response>
    /// <response code="400">If the building information does not pass validation.</response>
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

    /// <summary>Update a building.</summary>
    /// <response code="200">If the building is updated, together with the location.</response>
    /// <response code="400">If the building information does not pass validation.</response>
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
