namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity;

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
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("capacities")]
public class CapacityController : OrganisationRegistryController
{
    public CapacityController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available capacities.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<CapacityListQuery.CapacityListFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedCapacities = new CapacityListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedCapacities.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedCapacities.Items.ToListAsync());
    }

    /// <summary>Get a capacity.</summary>
    /// <response code="200">If the capacity is found.</response>
    /// <response code="404">If the capacity cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.CapacityList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
