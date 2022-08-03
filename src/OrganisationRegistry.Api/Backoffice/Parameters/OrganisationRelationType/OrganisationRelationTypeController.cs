namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.OrganisationRelationType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisationrelationtypes")]
public class OrganisationRelationTypeController : OrganisationRegistryController
{
    /// <summary>Get a list of available organisation relation types.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationRelationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisationRelationTypes = new OrganisationRelationTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationRelationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationRelationTypes.Items.ToListAsync());
    }

    /// <summary>Get an organisation relation type.</summary>
    /// <response code="200">If the organisation relation type is found.</response>
    /// <response code="404">If the organisation relation type cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.OrganisationRelationTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
