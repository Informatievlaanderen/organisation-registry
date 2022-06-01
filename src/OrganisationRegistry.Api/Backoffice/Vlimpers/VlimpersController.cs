namespace OrganisationRegistry.Api.Backoffice.Vlimpers;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using Security;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
public class VlimpersController : OrganisationRegistryController
{
    public VlimpersController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Couple an organisation to a KBO number.</summary>
    /// <response code="200">If the organisation was coupled.</response>
    [HttpPatch("{id}/vlimpers")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder + "," + Roles.Developer)]
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
