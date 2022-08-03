namespace OrganisationRegistry.Api.Backoffice.Organisation.Location;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/locations")]
public class OrganisationLocationController : OrganisationRegistryController
{
    /// <summary>Get a list of available locations for an organisation.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromServices] IMemoryCaches memoryCaches,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationLocationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisations = new OrganisationLocationListQuery(
            context,
            memoryCaches,
            organisationId,
            await securityService.GetUser(User)
        ).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a location for an organisation.</summary>
    /// <response code="200">If the location is found.</response>
    /// <response code="404">If the location cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationLocationList.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.OrganisationLocationId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }
}
