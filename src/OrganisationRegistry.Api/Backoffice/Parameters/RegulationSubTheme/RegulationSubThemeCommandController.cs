namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;
using Security;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("regulationsubthemes")]
[OrganisationRegistryAuthorize]
public class RegulationSubThemeCommandController : OrganisationRegistryController
{
    public RegulationSubThemeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create an regulation sub-theme.</summary>
    /// <response code="201">If the regulation sub-theme is created, together with the location.</response>
    /// <response code="400">If the regulation sub-theme information does not pass validation.</response>
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

    /// <summary>Update an regulation sub-theme.</summary>
    /// <response code="200">If the regulation sub-theme is updated, together with the location.</response>
    /// <response code="400">If the regulation sub-theme information does not pass validation.</response>
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
