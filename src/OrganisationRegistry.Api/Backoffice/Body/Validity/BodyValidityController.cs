namespace OrganisationRegistry.Api.Backoffice.Body.Validity;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyValidityController : OrganisationRegistryController
{
    /// <summary>Vraag de geldigheid van een orgaan op.</summary>
    /// <response code="200">Als het orgaan gevonden is.</response>
    /// <response code="404">Als het orgaan niet gevonden kan worden.</response>
    [HttpGet("{id}/validity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var body = await context.BodyDetail.FirstOrDefaultAsync(x => x.Id == id);

        if (body == null)
            return NotFound();

        return Ok(new BodyValidityResponse(body));
    }
}
