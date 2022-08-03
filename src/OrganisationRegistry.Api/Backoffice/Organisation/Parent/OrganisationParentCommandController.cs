namespace OrganisationRegistry.Api.Backoffice.Organisation.Parent;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/parents")]
[OrganisationRegistryAuthorize]
public class OrganisationParentCommandController : OrganisationRegistryCommandController
{
    public OrganisationParentCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a parent for an organisation.</summary>
    /// <response code="201">If the parent is created, together with the location.</response>
    /// <response code="400">If the parent information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationParentRequest message)
    {
        var internalMessage = new AddOrganisationParentInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationParentRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationParentController), nameof(OrganisationParentController.Get), new { id = message.OrganisationOrganisationParentId });
    }

    /// <summary>Update a parent for an organisation.</summary>
    /// <response code="201">If the parent is updated, together with the location.</response>
    /// <response code="400">If the parent information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationParentRequest message)
    {
        var internalMessage = new UpdateOrganisationParentInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationParentRequestMapping.Map(internalMessage));

        return Ok();
    }
}
