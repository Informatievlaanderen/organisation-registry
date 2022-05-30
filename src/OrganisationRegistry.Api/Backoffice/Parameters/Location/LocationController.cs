namespace OrganisationRegistry.Api.Backoffice.Parameters.Location;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Commands;
using Queries;
using Requests;
using Responses;
using Security;
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

    /// <summary>Create a location.</summary>
    /// <response code="201">If the location is created, together with the location.</response>
    /// <response code="400">If the location information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateLocationRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateLocationRequestMapping.Map(message));

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }

    /// <summary>Update a location.</summary>
    /// <response code="200">If the location is updated, together with the location.</response>
    /// <response code="400">If the location information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLocationRequest message)
    {
        var internalMessage = new UpdateLocationInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateLocationRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.LocationId });
    }
}