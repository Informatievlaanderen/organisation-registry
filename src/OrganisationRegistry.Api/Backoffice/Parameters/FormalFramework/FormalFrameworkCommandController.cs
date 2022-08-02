namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework;

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
[OrganisationRegistryRoute("formalframeworks")]
[OrganisationRegistryAuthorize]
public class FormalFrameworkCommandController : OrganisationRegistryController
{
    public FormalFrameworkCommandController(
        ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a formal framework.</summary>
    /// <response code="201">If the formal framework is created, together with the location.</response>
    /// <response code="400">If the formal framework information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateFormalFrameworkRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateFormalFrameworkRequestMapping.Map(message);
        await CommandSender.Send(command);

        return CreatedWithLocation(nameof(FormalFrameworkController), nameof(FormalFrameworkController.Get), new { id = command.Id });
    }

    /// <summary>Update a formal framework.</summary>
    /// <response code="200">If the formal framework is updated, together with the location.</response>
    /// <response code="400">If the formal framework information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateFormalFrameworkRequest message)
    {
        var internalMessage = new UpdateFormalFrameworkInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateFormalFrameworkRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(FormalFrameworkController), nameof(FormalFrameworkController.Get), new { id = internalMessage.FormalFrameworkId });
    }
}
