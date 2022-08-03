namespace OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/lifecyclephases")]
[OrganisationRegistryAuthorize]
public class BodyLifecyclePhaseCommandController : OrganisationRegistryCommandController
{
    public BodyLifecyclePhaseCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a lifecycle phase for a body.</summary>
    /// <response code="201">If the lifecycle phase is created, together with the location.</response>
    /// <response code="400">If the lifecycle phase information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyLifecyclePhaseRequest message)
    {
        var internalMessage = new AddBodyLifecyclePhaseInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyLifecyclePhaseRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodyLifecyclePhaseController), nameof(BodyLifecyclePhaseController.Get), new { id = message.BodyLifecyclePhaseId });
    }

    /// <summary>Update a lifecycle phase for a body.</summary>
    /// <response code="201">If the lifecycle phase is updated, together with the location.</response>
    /// <response code="400">If the lifecycle phase information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyLifecyclePhaseRequest message)
    {
        var internalMessage = new UpdateBodyLifecyclePhaseInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyLifecyclePhaseRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyLifecyclePhaseController), nameof(BodyLifecyclePhaseController.Get), new { id = internalMessage.BodyId });
    }
}
