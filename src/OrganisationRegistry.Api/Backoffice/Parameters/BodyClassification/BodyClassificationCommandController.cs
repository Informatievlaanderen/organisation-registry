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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class BodyClassificationCommandController : OrganisationRegistryCommandController
{
    public BodyClassificationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een orgaanclassificatie.</summary>
    /// <response code="201">Als de orgaanclassificatie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de orgaanclassificatie mislukt is.</response>
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

    /// <summary>Pas een orgaanclassificatie aan.</summary>
    /// <response code="200">Als de orgaanclassificatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de orgaanclassificatie mislukt is.</response>
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
