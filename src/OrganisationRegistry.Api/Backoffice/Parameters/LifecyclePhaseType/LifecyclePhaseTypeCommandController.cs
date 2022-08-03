namespace OrganisationRegistry.Api.Backoffice.Parameters.LifecyclePhaseType;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("lifecyclephasetypes")]
[OrganisationRegistryAuthorize]
public class LifecyclePhaseTypeCommandController : OrganisationRegistryCommandController
{
    public LifecyclePhaseTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a lifecyclephase type.</summary>
    /// <response code="201">If the lifecyclephase type is created, together with the location.</response>
    /// <response code="400">If the lifecyclephase type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateLifecyclePhaseTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateLifecyclePhaseTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(LifecyclePhaseTypeController), nameof(LifecyclePhaseTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a lifecyclephase type.</summary>
    /// <response code="200">If the lifecyclephase type is updated, together with the location.</response>
    /// <response code="400">If the lifecyclephase type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLifecyclePhaseTypeRequest message)
    {
        var internalMessage = new UpdateLifecyclePhaseTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateLifecyclePhaseTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(LifecyclePhaseTypeController), nameof(LifecyclePhaseTypeController.Get), new { id = internalMessage.LifecyclePhaseTypeId });
    }
}
