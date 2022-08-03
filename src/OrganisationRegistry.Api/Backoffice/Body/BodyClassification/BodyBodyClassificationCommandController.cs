namespace OrganisationRegistry.Api.Backoffice.Body.BodyClassification;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/classifications")]
[OrganisationRegistryAuthorize]
public class BodyBodyClassificationCommandController : OrganisationRegistryCommandController
{
    public BodyBodyClassificationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a classification for a body.</summary>
    /// <response code="201">If the classification is created, together with the location.</response>
    /// <response code="400">If the classification information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyBodyClassificationRequest message)
    {
        var internalMessage = new AddBodyBodyClassificationInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyBodyClassificationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodyBodyClassificationController), nameof(BodyBodyClassificationController.Get), new { id = message.BodyBodyClassificationId });
    }

    /// <summary>Update a classification for a body.</summary>
    /// <response code="201">If the classification is updated, together with the location.</response>
    /// <response code="400">If the classification information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyBodyClassificationRequest message)
    {
        var internalMessage = new UpdateBodyBodyClassificationInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyBodyClassificationRequestMapping.Map(internalMessage));

        return Ok();
    }
}
