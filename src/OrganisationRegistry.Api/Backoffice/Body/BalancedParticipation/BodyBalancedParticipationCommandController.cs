namespace OrganisationRegistry.Api.Backoffice.Body.BalancedParticipation;

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
public class BodyBalancedParticipationCommandController : OrganisationRegistryCommandController
{
    public BodyBalancedParticipationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Update a body's balanced participation info.</summary>
    /// <response code="200">If the body balanced participation info is updated, together with the location.</response>
    /// <response code="400">If the body balanced participation information does not pass validation.</response>
    [HttpPut("{id}/balancedparticipation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyBalancedParticipationRequest message)
    {
        var internalMessage = new UpdateBodyBalancedParticipationInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyBalancedParticipationRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyBalancedParticipationController), nameof(BodyBalancedParticipationController.Get), new { id = internalMessage.BodyId });
    }
}
