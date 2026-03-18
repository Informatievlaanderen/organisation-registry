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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyFormalFrameworkCommandController : OrganisationRegistryCommandController
{
    public BodyFormalFrameworkCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een toepassingsgebied toe aan een orgaan.</summary>
    /// <response code="201">Als het toepassingsgebied succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
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

    /// <summary>Pas een toepassingsgebied aan voor een orgaan.</summary>
    /// <response code="200">Als het toepassingsgebied succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
    /// <response code="200">Als het toepassingsgebied succesvol aangepast is.</response>
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
