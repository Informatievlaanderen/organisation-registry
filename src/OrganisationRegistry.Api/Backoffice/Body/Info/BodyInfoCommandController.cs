namespace OrganisationRegistry.Api.Backoffice.Body.Info;

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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyInfoCommandController : OrganisationRegistryCommandController
{
    public BodyInfoCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Pas de informatie van een orgaan aan.</summary>
    /// <response code="200">Als de orgaaninfo succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het orgaan mislukt is.</response>
    [HttpPut("{id}/info")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyInfoRequest message)
    {
        var internalMessage = new UpdateBodyInfoInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyInfoRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(BodyInfoController), nameof(BodyInfoController.Get), new { id = internalMessage.BodyId });
    }
}
