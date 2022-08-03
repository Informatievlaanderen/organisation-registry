namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType;

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
using SqlServer.FunctionType;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("functiontypes")]
public class FunctionTypeController : OrganisationRegistryController
{
    /// <summary>Get a list of available function types.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FunctionTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFunctionTypes = new FunctionTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFunctionTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFunctionTypes.Items.ToListAsync());
    }

    /// <summary>Get a function type.</summary>
    /// <response code="200">If the function type is found.</response>
    /// <response code="404">If the function type cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var functionTypeListItem = await context.FunctionTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (functionTypeListItem == null)
            return NotFound();

        return Ok(functionTypeListItem);
    }
}
