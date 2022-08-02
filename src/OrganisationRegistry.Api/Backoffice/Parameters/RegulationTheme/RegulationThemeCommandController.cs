namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationTheme;

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
[OrganisationRegistryRoute("regulationthemes")]
[OrganisationRegistryAuthorize]
public class RegulationThemeCommandController : OrganisationRegistryController
{
    public RegulationThemeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a regulation theme.</summary>
    /// <response code="201">If the regulation theme is created, together with the location.</response>
    /// <response code="400">If the regulation theme information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateRegulationThemeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateRegulationThemeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(RegulationThemeController), nameof(RegulationThemeController.Get), new { id = message.Id });
    }

    /// <summary>Update a regulation theme.</summary>
    /// <response code="200">If the regulation theme is updated, together with the location.</response>
    /// <response code="400">If the regulation theme information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRegulationThemeRequest message)
    {
        var internalMessage = new UpdateRegulationThemeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateRegulationThemeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(RegulationThemeController), nameof(RegulationThemeController.Get), new { id = internalMessage.RegulationThemeId });
    }
}
