namespace OrganisationRegistry.Api.Backoffice.Organisation.Relation;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/relations")]
public class OrganisationRelationController : OrganisationRegistryController
{
    /// <summary>Get a list of available relations for an organisation.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationRelationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisations = new OrganisationRelationListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a relation for an organisation.</summary>
    /// <response code="200">If the relation is found.</response>
    /// <response code="404">If the relation cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationRelationList.FirstOrDefaultAsync(x => x.OrganisationRelationId == id);

        if (organisation == null)
            return NotFound();

        return Ok(new OrganisationRelationResponse(organisation));
    }
}
