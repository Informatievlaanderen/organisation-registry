namespace OrganisationRegistry.Api.Backoffice.Body.Organisation;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/organisations")]
[OrganisationRegistryAuthorize]
public class BodyOrganisationCommandController : OrganisationRegistryCommandController
{
    public BodyOrganisationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Link an organisation to a body.</summary>
    /// <response code="201">If the organisation is linked, together with the location.</response>
    /// <response code="400">If the organisation information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyOrganisationRequest message)
    {
        var internalMessage = new AddBodyOrganisationInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyOrganisationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodyOrganisationController), nameof(BodyOrganisationController.Get), new { id = message.BodyOrganisationId });
    }

    /// <summary>Update an organisation for a body.</summary>
    /// <response code="201">If the organisation is updated, together with the location.</response>
    /// <response code="400">If the organisation information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyOrganisationRequest message)
    {
        var internalMessage = new UpdateBodyOrganisationInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyOrganisationRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyOrganisationController), nameof(BodyOrganisationController.Get), new { id = internalMessage.BodyId });
    }
}
