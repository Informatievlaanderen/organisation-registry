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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyMandateCommandController : OrganisationRegistryCommandController
{
    public BodyMandateCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een mandaat toe aan een orgaan.</summary>
    /// <response code="201">Als het mandaat succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het mandaat mislukt is.</response>
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

    /// <summary>Pas een mandaat aan voor een orgaan.</summary>
    /// <response code="201">Als het mandaat succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het mandaat mislukt is.</response>
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
