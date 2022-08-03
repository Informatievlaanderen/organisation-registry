namespace OrganisationRegistry.Api.Backoffice.Organisation.OrganisationClassification;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/classifications")]
[OrganisationRegistryAuthorize]
public class OrganisationOrganisationClassificationCommandController : OrganisationRegistryCommandController
{
    public OrganisationOrganisationClassificationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a classification for an organisation.</summary>
    /// <response code="201">If the classification is created, together with the location.</response>
    /// <response code="400">If the classification information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationOrganisationClassificationRequest message)
    {
        var internalMessage = new AddOrganisationOrganisationClassificationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationOrganisationClassificationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationOrganisationClassificationController), nameof(OrganisationOrganisationClassificationController.Get), new { id = message.OrganisationOrganisationClassificationId });
    }

    /// <summary>Update a classification for an organisation.</summary>
    /// <response code="201">If the classification is updated, together with the location.</response>
    /// <response code="400">If the classification information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationOrganisationClassificationRequest message)
    {
        var internalMessage = new UpdateOrganisationOrganisationClassificationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationOrganisationClassificationRequestMapping.Map(internalMessage));

        return Ok();
    }
}
