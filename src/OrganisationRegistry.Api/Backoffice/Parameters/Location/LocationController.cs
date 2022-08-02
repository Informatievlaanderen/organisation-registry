namespace OrganisationRegistry.Api.Backoffice.Parameters.Location;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Commands;
using Queries;
using Responses;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("locations")]
public class LocationController : OrganisationRegistryController
{
    public LocationController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available location types.</summary>
    [HttpGet]
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

    /// <summary>Get a location type.</summary>
    /// <response code="200">If the location type is found.</response>
    /// <response code="404">If the location type cannot be found.</response>
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
