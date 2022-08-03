namespace OrganisationRegistry.Api.Backoffice.Organisation.Label;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/labels")]
[OrganisationRegistryAuthorize]
public class OrganisationLabelCommandController : OrganisationRegistryCommandController
{
    public OrganisationLabelCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a label for an organisation.</summary>
    /// <response code="201">If the label is created, together with the label.</response>
    /// <response code="400">If the label information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationLabelRequest message)
    {
        var internalMessage = new AddOrganisationLabelInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationLabelRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationLabelController), nameof(OrganisationLabelController.Get), new { id = message.OrganisationLabelId });
    }

    /// <summary>Update a label for an organisation.</summary>
    /// <response code="201">If the label is updated, together with the label.</response>
    /// <response code="400">If the label information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationLabelRequest message)
    {
        var internalMessage = new UpdateOrganisationLabelInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationLabelRequestMapping.Map(internalMessage));

        return Ok();
    }
}
