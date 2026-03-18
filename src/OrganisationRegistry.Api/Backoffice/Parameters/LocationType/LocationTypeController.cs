namespace OrganisationRegistry.Api.Backoffice.Parameters.LocationType;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Configuration;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.LocationType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("locationtypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class LocationTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van locatietypes op.</summary>
    /// <response code="200">Een lijst van locatietypes.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration)
    {
        var filtering = Request.ExtractFilteringRequest<LocationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLocationTypes = new LocationTypeListQuery(context, configuration).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLocationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLocationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een locatietype op.</summary>
    /// <response code="200">Als het locatietype gevonden is.</response>
    /// <response code="404">Als het locatietype niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.LocationTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
