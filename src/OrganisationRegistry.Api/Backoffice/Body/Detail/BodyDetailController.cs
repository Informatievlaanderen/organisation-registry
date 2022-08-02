namespace OrganisationRegistry.Api.Backoffice.Body.Detail;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
public class BodyDetailController : OrganisationRegistryController
{
    public BodyDetailController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a body.</summary>
    /// <response code="200">If the body is found.</response>
    /// <response code="404">If the body cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var body =
            await context.BodyDetail
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

        if (body == null)
            return NotFound();

        var hasAllSeatsAssigned = BodyParticipationStatus.HasAllSeatsAssigned(context, id);

        return Ok(new BodyResponse(body, hasAllSeatsAssigned, BodyParticipationStatus.IsMepCompliant(context, id)));
    }
}
