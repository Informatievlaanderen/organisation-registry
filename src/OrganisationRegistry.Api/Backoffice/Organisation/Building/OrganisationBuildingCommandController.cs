namespace OrganisationRegistry.Api.Backoffice.Organisation.Building;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/buildings")]
[OrganisationRegistryAuthorize]
public class OrganisationBuildingCommandController : OrganisationRegistryCommandController
{
    public OrganisationBuildingCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Add a building to an organisation.</summary>
    /// <response code="201">If the building is added, together with the location.</response>
    /// <response code="400">If the building information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationBuildingRequest message)
    {
        var internalMessage = new AddOrganisationBuildingInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationBuildingRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationBuildingController), nameof(OrganisationBuildingController.Get), new { id = message.OrganisationBuildingId });
    }

    /// <summary>Update a building for an organisation.</summary>
    /// <response code="201">If the building is updated, together with the location.</response>
    /// <response code="400">If the building information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationBuildingRequest message)
    {
        var internalMessage = new UpdateOrganisationBuildingInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationBuildingRequestMapping.Map(internalMessage));

        return Ok();
    }
}
