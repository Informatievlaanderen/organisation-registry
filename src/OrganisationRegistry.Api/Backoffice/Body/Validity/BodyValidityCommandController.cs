namespace OrganisationRegistry.Api.Backoffice.Body.Validity;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
[OrganisationRegistryAuthorize]
public class BodyValidityCommandController : OrganisationRegistryCommandController
{
    public BodyValidityCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Update a body's validity.</summary>
    /// <response code="200">If the body validity is updated, together with the location.</response>
    /// <response code="400">If the body validity information does not pass validation.</response>
    [HttpPut("{id}/validity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyValidityRequest message)
    {
        var internalMessage = new UpdateBodyValidityInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyValidityRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyValidityController), nameof(BodyValidityController.Get), new { id = internalMessage.BodyId });
    }
}
