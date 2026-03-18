namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType;

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
[OrganisationRegistryRoute("mandateroletypes")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class MandateRoleTypeCommandController : OrganisationRegistryCommandController
{
    public MandateRoleTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een mandaat rol type.</summary>
    /// <response code="201">Als het mandaat rol type succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het mandaat rol type mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateMandateRoleTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateMandateRoleTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(MandateRoleTypeController), nameof(MandateRoleTypeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een mandaat rol type aan.</summary>
    /// <response code="200">Als het mandaat rol type succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het mandaat rol type mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateMandateRoleTypeRequest message)
    {
        var internalMessage = new UpdateMandateRoleTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateMandateRoleTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(MandateRoleTypeController), nameof(MandateRoleTypeController.Get), new { id = internalMessage.MandateRoleTypeId });
    }
}
