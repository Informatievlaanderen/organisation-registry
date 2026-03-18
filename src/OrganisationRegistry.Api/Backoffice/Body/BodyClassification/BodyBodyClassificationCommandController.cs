namespace OrganisationRegistry.Api.Backoffice.Body.BodyClassification;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/classifications")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyBodyClassificationCommandController : OrganisationRegistryCommandController
{
    public BodyBodyClassificationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een classificatie toe aan een orgaan.</summary>
    /// <response code="201">Als de classificatie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de classificatie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid bodyId, [FromBody] AddBodyBodyClassificationRequest message)
    {
        var internalMessage = new AddBodyBodyClassificationInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyBodyClassificationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(BodyBodyClassificationController), nameof(BodyBodyClassificationController.Get), new { id = message.BodyBodyClassificationId });
    }

    /// <summary>Pas een classificatie aan voor een orgaan.</summary>
    /// <response code="200">Als de classificatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de classificatie mislukt is.</response>
    /// <response code="200">Als de orgaanclassificatie succesvol aangepast is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid bodyId, [FromBody] UpdateBodyBodyClassificationRequest message)
    {
        var internalMessage = new UpdateBodyBodyClassificationInternalRequest(bodyId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyBodyClassificationRequestMapping.Map(internalMessage));

        return Ok();
    }
}
