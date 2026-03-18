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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyLifecyclePhaseCommandController : OrganisationRegistryCommandController
{
    public BodyLifecyclePhaseCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een levensloopfase toe aan een orgaan.</summary>
    /// <response code="201">Als de levensloopfase succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de levensloopfase mislukt is.</response>
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

    /// <summary>Pas een levensloopfase aan voor een orgaan.</summary>
    /// <response code="200">Als de levensloopfase succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de levensloopfase mislukt is.</response>
    /// <response code="200">Als de levensloopfase succesvol aangepast is.</response>
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
