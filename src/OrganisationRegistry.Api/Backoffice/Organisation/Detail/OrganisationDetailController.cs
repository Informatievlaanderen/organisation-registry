namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Commands;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
public class OrganisationDetailController : OrganisationRegistryController
{
    public OrganisationDetailController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Get an organisation.</summary>
    /// <response code="200">If the organisation is found.</response>
    /// <response code="404">If the organisation cannot be found.</response>
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
