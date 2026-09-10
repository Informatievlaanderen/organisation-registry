namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
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
    public async Task<IActionResult> GetById([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id,
        [FromServices] ISecurityService securityService,
        [FromServices] IMemoryCaches memoryCaches)
    {
        var organisation = await context.OrganisationDetail.FirstOrDefaultAsync(x => x.Id == id);

        if (organisation == null)
            return NotFound();

        var user = await securityService.GetUser(User);

        OrganisationPermissions PermissionsFactory(string ovoNumber, Guid organisationId)
            => OrganisationPermissions.For(
                user,
                ovoNumber,
                memoryCaches.UnderVlimpersManagement.Contains(organisationId));

        return Ok(new OrganisationResponse(organisation, PermissionsFactory));
    }

    /// <summary>Vraag een organisatie op basis van OVO-nummer op.</summary>
    /// <response code="200">Als de organisatie gevonden is.</response>
    /// <response code="404">Als de organisatie niet gevonden kan worden.</response>
    [HttpGet("{ovoNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOvoNumber([FromServices] OrganisationRegistryContext context, [FromRoute] string ovoNumber,
        [FromServices] ISecurityService securityService,
        [FromServices] IMemoryCaches memoryCaches)
    {
        var organisation = await context.OrganisationDetail.FirstOrDefaultAsync(x => x.OvoNumber == ovoNumber);

        if (organisation == null)
            return NotFound();

        var user = await securityService.GetUser(User);

        OrganisationPermissions PermissionsFactory(string ovo, Guid organisationId)
            => OrganisationPermissions.For(
                user,
                ovo,
                memoryCaches.UnderVlimpersManagement.Contains(organisationId));

        return Ok(new OrganisationResponse(organisation, PermissionsFactory));
    }
}
