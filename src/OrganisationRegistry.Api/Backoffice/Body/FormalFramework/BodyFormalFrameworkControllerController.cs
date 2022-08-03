namespace OrganisationRegistry.Api.Backoffice.Body.FormalFramework;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/formalframeworks")]
[OrganisationRegistryAuthorize]
public class BodyFormalFrameworkCommandController : OrganisationRegistryCommandController
{
    public BodyFormalFrameworkCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a formal framework for a body.</summary>
    /// <response code="201">If the formal framework is created, together with the location.</response>
    /// <response code="400">If the formal framework information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyFormalFrameworkRequest message)
    {
        var internalMessage = new AddBodyFormalFrameworkInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyFormalFrameworkRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodyFormalFrameworkController), nameof(BodyFormalFrameworkController.Get), new { id = message.BodyFormalFrameworkId });
    }

    /// <summary>Update a formal framework for a body.</summary>
    /// <response code="201">If the formal framework is updated, together with the location.</response>
    /// <response code="400">If the formal framework information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyFormalFrameworkRequest message)
    {
        var internalMessage = new UpdateBodyFormalFrameworkInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyFormalFrameworkRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyFormalFrameworkController), nameof(BodyFormalFrameworkController.Get), new { id = internalMessage.BodyId });
    }
}
