namespace OrganisationRegistry.Api.Backoffice.Body.Validity;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyValidityCommandController : OrganisationRegistryCommandController
{
    public BodyValidityCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Pas de geldigheid van een orgaan aan.</summary>
    /// <response code="200">Als de orgaangeldigheid succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de orgaangeldigheid mislukt is.</response>
    [HttpPut("{id}/validity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyValidityRequest message)
    {
        var internalMessage = new UpdateBodyValidityInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyValidityRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyValidityController), nameof(BodyValidityController.Get), new { id = internalMessage.BodyId });
    }
}
