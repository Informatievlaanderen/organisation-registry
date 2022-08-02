namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassification;

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
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("bodyclassifications")]
public class BodyClassificationCommandController : OrganisationRegistryController
{
    public BodyClassificationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create an body classification.</summary>
    /// <response code="201">If the body classificiation is created, together with the location.</response>
    /// <response code="400">If the body classificiation information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBodyClassificationRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateBodyClassificationRequestMapping.Map(message));

        return CreatedWithLocation(nameof(BodyClassificationController),nameof(BodyClassificationController.Get), new { id = message.Id });
    }

    /// <summary>Update an body classification.</summary>
    /// <response code="200">If the body classification is updated, together with the location.</response>
    /// <response code="400">If the body classification information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyClassificationRequest message)
    {
        var internalMessage = new UpdateBodyClassificationInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyClassificationRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyClassificationController),nameof(BodyClassificationController.Get), new { id = internalMessage.BodyClassificationId });
    }
}
