namespace OrganisationRegistry.Api.Backoffice.Parameters.Building;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queries;
using SqlServer.Building;
using SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("buildings")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class BuildingController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van gebouwen op.</summary>
    /// <response code="200">Een lijst van gebouwen.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingListExamples))]
    [ProducesResponseType(typeof(List<BuildingListItem>), StatusCodes.Status200OK)]
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

    /// <summary>Vraag een gebouw op.</summary>
    /// <response code="200">Als het gebouw gevonden is.</response>
    /// <response code="404">Als het gebouw niet gevonden kan worden.</response>
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
