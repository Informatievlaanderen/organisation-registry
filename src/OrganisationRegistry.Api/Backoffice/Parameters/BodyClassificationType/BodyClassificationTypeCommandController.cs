namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassificationType;

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
[OrganisationRegistryRoute("bodyclassificationtypes")]
public class BodyClassificationTypeCommandController : OrganisationRegistryCommandController
{
    public BodyClassificationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create an body classification type.</summary>
    /// <response code="201">If the body classificiation type is created, together with the location.</response>
    /// <response code="400">If the body classificiation type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBodyClassificationTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateBodyClassificationTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(BodyClassificationTypeController),nameof(BodyClassificationTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update an body classification type.</summary>
    /// <response code="200">If the body classification type is updated, together with the location.</response>
    /// <response code="400">If the body classification type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyClassificationTypeRequest message)
    {
        var internalMessage = new UpdateBodyClassificationTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyClassificationTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyClassificationTypeController),nameof(BodyClassificationTypeController.Get), new { id = internalMessage.BodyClassificationTypeId });
    }
}
