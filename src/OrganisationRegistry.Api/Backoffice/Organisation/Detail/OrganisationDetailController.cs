namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationDetailController : OrganisationRegistryController
{
    /// <summary>Vraag een organisatie op.</summary>
    /// <response code="200">Als de organisatie gevonden is.</response>
    /// <response code="404">Als de organisatie niet gevonden kan worden.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationDetail.FirstOrDefaultAsync(x => x.Id == id);

        if (organisation == null)
            return NotFound();

        return Ok(new OrganisationResponse(organisation));
    }

    /// <summary>Vraag een organisatie op basis van OVO-nummer op.</summary>
    /// <response code="200">Als de organisatie gevonden is.</response>
    /// <response code="404">Als de organisatie niet gevonden kan worden.</response>
    [HttpGet("{ovoNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] string ovoNumber)
    {
        var organisation = await context.OrganisationDetail.FirstOrDefaultAsync(x => x.OvoNumber == ovoNumber);

        if (organisation == null)
            return NotFound();

        return Ok(new OrganisationResponse(organisation));
    }
}
