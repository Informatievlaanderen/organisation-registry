namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType;

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
[OrganisationRegistryRoute("labeltypes")]
[OrganisationRegistryAuthorize]
public class LabelTypeCommandController : OrganisationRegistryController
{
    public LabelTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a label type.</summary>
    /// <response code="201">If the label type is created, together with the location.</response>
    /// <response code="400">If the label type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateLabelTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateLabelTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(LabelTypeController), nameof(LabelTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a label type.</summary>
    /// <response code="200">If the label type is updated, together with the location.</response>
    /// <response code="400">If the label type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLabelTypeRequest message)
    {
        var internalMessage = new UpdateLabelTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateLabelTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(LabelTypeController), nameof(LabelTypeController.Get), new { id = internalMessage.LabelTypeId });
    }
}
