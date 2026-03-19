namespace OrganisationRegistry.Api.Backoffice.Organisation.Key;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/keys")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationKeyController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van sleutels voor een organisatie op.</summary>
    /// <response code="200">Een lijst van sleutels.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrganisationKeyListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationKeyListExamples))]
    [OrganisationRegistryAuthorize]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationKeyListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForKeyType = keyTypeId => new KeyPolicy(
                memoryCaches.OvoNumbers[organisationId],
                configuration,
                keyTypeId)
            .Check(user)
            .IsSuccessful;

        var pagedOrganisations = new OrganisationKeyListQuery(context, organisationId, isAuthorizedForKeyType).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Vraag een sleutel voor een organisatie op.</summary>
    /// <response code="200">Als de sleutel gevonden is.</response>
    /// <response code="404">Als de sleutel niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisationKey = await context.OrganisationKeyList.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.OrganisationKeyId == id);

        if (organisationKey == null)
            return NotFound();

        return Ok(organisationKey);
    }
}
