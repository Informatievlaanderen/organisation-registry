namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme;

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
[OrganisationRegistryRoute("regulationsubthemes")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class RegulationSubThemeCommandController : OrganisationRegistryCommandController
{
    public RegulationSubThemeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een regelgevingsubthema.</summary>
    /// <response code="201">Als het regelgevingsubthema succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het regelgevingsubthema mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateRegulationSubThemeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateRegulationSubThemeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(RegulationSubThemeController), nameof(RegulationSubThemeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een regelgevingsubthema aan.</summary>
    /// <response code="200">Als het regelgevingsubthema succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het regelgevingsubthema mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRegulationSubThemeRequest message)
    {
        var internalMessage = new UpdateRegulationSubThemeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateRegulationSubThemeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(RegulationSubThemeController), nameof(RegulationSubThemeController.Get), new { id = internalMessage.RegulationSubThemeId });
    }
}
