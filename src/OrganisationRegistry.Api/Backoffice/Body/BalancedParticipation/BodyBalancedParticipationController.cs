namespace OrganisationRegistry.Api.Backoffice.Body.BalancedParticipation;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.OrgaanBeheerder, Role.CjmBeheerder)]
public class BodyBalancedParticipationController : OrganisationRegistryController
{
    public BodyBalancedParticipationController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a body's balanced participation info.</summary>
    /// <response code="200">If the body is found.</response>
    /// <response code="404">If the body cannot be found.</response>
    [HttpGet("{id}/balancedparticipation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var body = await context.BodyDetail.FirstOrDefaultAsync(x => x.Id == id);

        if (body == null)
            return NotFound();

        return Ok(new BodyBalancedParticipationResponse(body));
    }
}
