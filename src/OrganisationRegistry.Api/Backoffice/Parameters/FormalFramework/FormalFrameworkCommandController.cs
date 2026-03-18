namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework;

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
[OrganisationRegistryRoute("formalframeworks")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class FormalFrameworkCommandController : OrganisationRegistryCommandController
{
    public FormalFrameworkCommandController(
        ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een toepassingsgebied.</summary>
    /// <response code="201">Als het toepassingsgebied succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateFormalFrameworkRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateFormalFrameworkRequestMapping.Map(message);
        await CommandSender.Send(command);

        return CreatedWithLocation(nameof(FormalFrameworkController), nameof(FormalFrameworkController.Get), new { id = command.Id });
    }

    /// <summary>Pas een toepassingsgebied aan.</summary>
    /// <response code="200">Als het toepassingsgebied succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateFormalFrameworkRequest message)
    {
        var internalMessage = new UpdateFormalFrameworkInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateFormalFrameworkRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(FormalFrameworkController), nameof(FormalFrameworkController.Get), new { id = internalMessage.FormalFrameworkId });
    }
}
