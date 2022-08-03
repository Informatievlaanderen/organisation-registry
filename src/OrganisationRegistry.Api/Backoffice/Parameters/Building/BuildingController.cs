namespace OrganisationRegistry.Api.Backoffice.Parameters.Building;

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

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("buildings")]
public class BuildingController : OrganisationRegistryController
{
    /// <summary>Get a list of available buildings.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<BuildingListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBuildings = new BuildingListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBuildings.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBuildings.Items.ToListAsync());
    }

    /// <summary>Get a building.</summary>
    /// <response code="200">If the building is found.</response>
    /// <response code="404">If the building cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var building = await context.BuildingList.FirstOrDefaultAsync(x => x.Id == id);

        if (building == null)
            return NotFound();

        return Ok(building);
    }
}
