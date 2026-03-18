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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class LabelTypeCommandController : OrganisationRegistryCommandController
{
    public LabelTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een labeltype.</summary>
    /// <response code="201">Als het benamingstype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het benamingstype mislukt is.</response>
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

    /// <summary>Pas een labeltype aan.</summary>
    /// <response code="200">Als het benamingstype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het benamingstype mislukt is.</response>
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
