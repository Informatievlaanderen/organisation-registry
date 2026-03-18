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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class BodyClassificationTypeCommandController : OrganisationRegistryCommandController
{
    public BodyClassificationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een orgaanclassificatietype.</summary>
    /// <response code="201">Als het orgaanclassificatietype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het orgaanclassificatietype mislukt is.</response>
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

    /// <summary>Pas een orgaanclassificatietype aan.</summary>
    /// <response code="200">Als het orgaanclassificatietype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het orgaanclassificatietype mislukt is.</response>
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
