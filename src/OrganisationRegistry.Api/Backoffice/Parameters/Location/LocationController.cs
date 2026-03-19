namespace OrganisationRegistry.Api.Backoffice.Parameters.Location;

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
using Responses;
using SqlServer.Infrastructure;
using SqlServer.Location;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("locations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class LocationController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van locaties op.</summary>
    /// <response code="200">Een lijst van locaties.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LocationListExamples))]
    [ProducesResponseType(typeof(List<LocationListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<LocationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLocations = new LocationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLocations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLocations.Items.ToListAsync());
    }

    /// <summary>Vraag een locatie op.</summary>
    /// <response code="200">Als de locatie gevonden is.</response>
    /// <response code="404">Als de locatie niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var location = await context.LocationList.FirstOrDefaultAsync(x => x.Id == id);

        if (location == null)
            return NotFound();

        return Ok(new LocationResponse(location));
    }
}
