namespace OrganisationRegistry.Api.Backoffice.Organisation.Relation;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/relations")]
[OrganisationRegistryAuthorize]
public class OrganisationRelationCommandController : OrganisationRegistryCommandController
{
    public OrganisationRelationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a relation for an organisation.</summary>
    /// <response code="201">If the relation is created.</response>
    /// <response code="400">If the relation information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationRelationRequest message)
    {
        var internalMessage = new AddOrganisationRelationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationRelationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationRelationController), nameof(OrganisationRelationController.Get), new { id = message.OrganisationRelationId });
    }

    /// <summary>Update a relation for an organisation.</summary>
    /// <response code="201">If the relation is updated.</response>
    /// <response code="400">If the relation information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationRelationRequest message)
    {
        var internalMessage = new UpdateOrganisationRelationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationRelationRequestMapping.Map(internalMessage));

        return Ok();
    }
}
