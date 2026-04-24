namespace OrganisationRegistry.Api.Backoffice.Vlimpers;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
[ApiController]
[OrganisationRegistryAuthorize]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class VlimpersController : OrganisationRegistryCommandController
{
    public VlimpersController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Pas aan of een organisatie onder Vlimpersbeheer valt of niet.</summary>
    /// <response code="200">Als de organisatie succesvol aangepast is.</response>
    [HttpPatch("{id}/vlimpers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Vlimpers(
        [FromRoute] Guid id,
        [FromBody] VlimpersRequest message)
    {
        if (message.VlimpersManagement)
        {
            await CommandSender.Send(
                new PlaceUnderVlimpersManagement(new OrganisationId(id)));
        }
        else
        {
            await CommandSender.Send(
                new ReleaseFromVlimpersManagement(new OrganisationId(id)));
        }

        return Ok();
    }
}
