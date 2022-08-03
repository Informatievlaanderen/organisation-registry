namespace OrganisationRegistry.Api.Backoffice.Body.Mandate;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/mandates")]
[OrganisationRegistryAuthorize]
public class BodyMandateCommandController : OrganisationRegistryCommandController
{
    public BodyMandateCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a mandate for a body.</summary>
    /// <response code="201">If the mandate is created, together with the location.</response>
    /// <response code="400">If the mandate information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyMandateRequest message)
    {
        var internalMessage = new AddBodyMandateInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        if (!internalMessage.Body.BodyMandateType.HasValue)
            return BadRequest("Body Mandate Type is required."); // NOTE: this should never happen.

        switch (internalMessage.Body.BodyMandateType.Value)
        {
            case BodyMandateType.Person:
                await CommandSender.Send(AddBodyMandateRequestMapping.MapForPerson(internalMessage));
                break;
            case BodyMandateType.FunctionType:
                await CommandSender.Send(AddBodyMandateRequestMapping.MapForFunctionType(internalMessage));
                break;
            case BodyMandateType.Organisation:
                await CommandSender.Send(AddBodyMandateRequestMapping.MapForOrganisation(internalMessage));
                break;
        }

        return CreatedWithLocation(nameof(BodyMandateController), nameof(BodyMandateController.Get), new { id = message.BodyMandateId });
    }

    /// <summary>Update a mandate for a body.</summary>
    /// <response code="201">If the mandate is updated, together with the location.</response>
    /// <response code="400">If the mandate information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyMandateRequest message)
    {
        var internalMessage = new UpdateBodyMandateInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        if (!internalMessage.Body.BodyMandateType.HasValue)
            return BadRequest("Body Mandate Type is required."); // NOTE: this should never happen.

        switch (internalMessage.Body.BodyMandateType.Value)
        {
            case BodyMandateType.Person:
                await CommandSender.Send(UpdateBodyMandateRequestMapping.MapForPerson(internalMessage));
                break;
            case BodyMandateType.FunctionType:
                await CommandSender.Send(UpdateBodyMandateRequestMapping.MapForFunctionType(internalMessage));
                break;
            case BodyMandateType.Organisation:
                await CommandSender.Send(UpdateBodyMandateRequestMapping.MapForOrganisation(internalMessage));
                break;
        }

        return OkWithLocationHeader(nameof(BodyMandateController), nameof(BodyMandateController.Get), new { id = internalMessage.BodyId });
    }
}
