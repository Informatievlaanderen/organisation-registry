namespace OrganisationRegistry.Api.Backoffice.Parameters.Purpose;

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
[OrganisationRegistryRoute("purposes")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class PurposeCommandController : OrganisationRegistryCommandController
{
    public PurposeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een doeleinde.</summary>
    /// <response code="201">Als het beleidsveld succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het beleidsveld mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreatePurposeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreatePurposeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(PurposeController), nameof(PurposeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een doeleinde aan.</summary>
    /// <response code="200">Als het beleidsveld succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het beleidsveld mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdatePurposeRequest message)
    {
        var internalMessage = new UpdatePurposeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdatePurposeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(PurposeController), nameof(PurposeController.Get), new { id = internalMessage.PurposeId });
    }
}
